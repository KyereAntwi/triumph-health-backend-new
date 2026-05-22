namespace Triumph.HealthMs.Persistence.Services;

public sealed class PatientUpsetService(
    IPatientManagementDbContext dbContext,
    IApplicationUserManagementDbContext applicationUserManagementDbContext,
    NpgsqlConnection connection,
    ILogger<PatientUpsetService> logger) : IPatientUpsetService
{
    public async Task<(string?, Guid?)> UpsetPatientDetails(AddPatientCommand command, CancellationToken cancellationToken)
    {
        var applicationUser = new ApplicationUser
        {
            Id = Guid.CreateVersion7(),
            UserId = Guid.NewGuid().ToString(),
            FirstName = command.FirstName,
            LastName = command.LastName,
            Gender = Enum.Parse<Gender>(command.Gender),
            Nationality = Enum.Parse<Nationality>(command.Nationality),
            DateOfBirth = DateOnly.Parse(command.DateOfBirth),
            OtherNames = command.OtherNames,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber
        };

        var patient = new Patient
        {
            Id = Guid.CreateVersion7(),
            ApplicationUserId = applicationUser.Id,
            UniqueIdentifier = "", // TODO - generate identifier
            Address = command.Address,
            PostGps = command.PostGps
        };

        if (command.Identifications != null && command.Identifications.Any())
        {
            foreach (var identification in command.Identifications)
            {
                patient.Identifications.Add(new Identification
                {
                    Type = Enum.Parse<IdentificationType>(identification.Type),
                    IdentificationNumber = identification.IdentificationNumber,
                    DateIssued = DateOnly.Parse(identification.DateIssued),
                    DateExpires = DateOnly.Parse(identification.DateExpires),
                    PlaceOfIssue = identification.PlaceOfIssue,
                    CountryOfIssue = identification.CountryOfIssue,
                    PatientId = patient.Id
                });
            }
        }

        var strategy = ((DbContext)dbContext).Database.CreateExecutionStrategy();

        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);
            
                await using var tx = await connection.BeginTransactionAsync(cancellationToken);
                await ((DbContext)applicationUserManagementDbContext).Database.UseTransactionAsync(tx, cancellationToken);
                await ((DbContext)dbContext).Database.UseTransactionAsync(tx, cancellationToken);
            
                await applicationUserManagementDbContext.ApplicationUsers.AddAsync(applicationUser, cancellationToken);

                if (command.SendAccountLinkageInvitation)
                {
                    var invitation = new LinkInvitation
                    {
                        Id = Guid.CreateVersion7(),
                        ApplicationUserId = applicationUser.Id,
                        InvitedEntityType = nameof(Employee)
                    };
                    await applicationUserManagementDbContext.LinkInvitations.AddAsync(invitation, cancellationToken);
                }
            
                await applicationUserManagementDbContext.SaveChangesAsync(cancellationToken);
                await dbContext.Patients.AddAsync(patient, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to persist patient. Command: {Command}", command);
            return ("An error occurred while adding the patient.", null);
        }
        finally
        {
            await connection.CloseAsync();
        }

        return (string.Empty, patient.Id);
    }

    public async Task<string?> UpsetForUpdatePatientDetails(UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        var patient = await dbContext
            .Patients
            .AsTracking()
            .FirstOrDefaultAsync(p => p.Id == command.PatientId, cancellationToken);

        patient!.PostGps = command.PostGps;
        patient.Address = command.Address;

        var userAccount =
            await applicationUserManagementDbContext
                .ApplicationUsers
                .AsTracking()
                .FirstOrDefaultAsync(a => a.Id == patient.ApplicationUserId, cancellationToken);

        userAccount!.FirstName = command.FirstName;
        userAccount.LastName = command.LastName;
        userAccount.Email = command.Email;
        userAccount.PhoneNumber = command.PhoneNumber;
        userAccount.OtherNames = command.OtherNames;
        userAccount.Gender = Enum.Parse<Gender>(command.Gender);
        userAccount.Nationality = Enum.Parse<Nationality>(command.Nationality);
        userAccount.DateOfBirth = DateOnly.Parse(command.DateOfBirth);

        var strategy = ((DbContext)dbContext).Database.CreateExecutionStrategy();
        try
        {
            await strategy.ExecuteAsync(async () =>
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

                await using var tx = await connection.BeginTransactionAsync(cancellationToken);
                await ((DbContext)applicationUserManagementDbContext).Database.UseTransactionAsync(tx, cancellationToken);
                await ((DbContext)dbContext).Database.UseTransactionAsync(tx, cancellationToken);

                dbContext.Patients.Update(patient);
                await dbContext.SaveChangesAsync(cancellationToken);
                applicationUserManagementDbContext.ApplicationUsers.Update(userAccount);
                await applicationUserManagementDbContext.SaveChangesAsync(cancellationToken);

                await tx.CommitAsync(cancellationToken);
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to persist patient update changes. Command : {Command}", command);
            return e.Message;
        }
        finally
        {
            await connection.CloseAsync();
        }

        return null;
    }
}