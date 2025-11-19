BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "AccessValidationHistory" (
	"Id"	INTEGER NOT NULL,
	"TesterId"	INTEGER NOT NULL,
	"CheckedAt"	TEXT NOT NULL,
	"IpAddress"	TEXT NOT NULL,
	CONSTRAINT "PK_AccessValidationHistory" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_AccessValidationHistory_Tester_TesterId" FOREIGN KEY("TesterId") REFERENCES "Tester"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "Tester" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT NOT NULL COLLATE NOCASE,
	"AccessKey"	TEXT,
	"CreatedAt"	TEXT NOT NULL,
	CONSTRAINT "PK_Tester" PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_CheckedAt" ON "AccessValidationHistory" (
	"CheckedAt"
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_IpAddress" ON "AccessValidationHistory" (
	"IpAddress"
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_TesterId" ON "AccessValidationHistory" (
	"TesterId"
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Tester_Name" ON "Tester" (
	"Name" COLLATE NOCASE
);
COMMIT;
