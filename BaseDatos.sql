IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Persons] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Age] int NOT NULL,
    [Identification] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Persons] PRIMARY KEY ([Id])
);

CREATE TABLE [Clients] (
    [Id] int NOT NULL,
    [ClientCode] nvarchar(450) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Clients_Persons_Id] FOREIGN KEY ([Id]) REFERENCES [Persons] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Accounts] (
    [Id] int NOT NULL IDENTITY,
    [AccountNumber] nvarchar(450) NOT NULL,
    [AccountType] nvarchar(max) NOT NULL,
    [InitialBalance] decimal(18,2) NOT NULL,
    [CurrentBalance] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    [ClientId] int NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Accounts_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Movements] (
    [Id] int NOT NULL IDENTITY,
    [AccountId] int NOT NULL,
    [OccurredAt] datetime2 NOT NULL,
    [MovementType] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [AvailableBalanceAfter] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Movements] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Movements_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Accounts_AccountNumber] ON [Accounts] ([AccountNumber]);

CREATE INDEX [IX_Accounts_ClientId] ON [Accounts] ([ClientId]);

CREATE UNIQUE INDEX [IX_Clients_ClientCode] ON [Clients] ([ClientCode]);

CREATE INDEX [IX_Movements_AccountId] ON [Movements] ([AccountId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250919201252_Initial', N'9.0.9');

ALTER TABLE [Clients] ADD [PasswordSalt] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250920041909_AddClientSalt', N'9.0.9');

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Clients]') AND [c].[name] = N'PasswordSalt');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Clients] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [Clients] DROP COLUMN [PasswordSalt];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250920214232_DropPasswordSalt', N'9.0.9');

COMMIT;
GO

