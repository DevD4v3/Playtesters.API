BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "AccessValidationHistory" (
	"Id"	INTEGER NOT NULL,
	"TesterId"	INTEGER NOT NULL,
	"CheckedAt"	TEXT NOT NULL,
	"IpAddress"	TEXT NOT NULL,
	"City" TEXT NOT NULL DEFAULT '' COLLATE NOCASE,
	"Country" TEXT NOT NULL DEFAULT '' COLLATE NOCASE,
	CONSTRAINT "PK_AccessValidationHistory" PRIMARY KEY("Id" AUTOINCREMENT),
	CONSTRAINT "FK_AccessValidationHistory_Tester_TesterId" FOREIGN KEY("TesterId") REFERENCES "Tester"("Id") ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS "IpGeoCache" (
	"Id"	INTEGER NOT NULL,
	"IpAddress"	TEXT NOT NULL,
	"Country"	TEXT NOT NULL,
	"City"	TEXT NOT NULL,
	CONSTRAINT "PK_IpGeoCache" PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "Tester" (
	"Id"	INTEGER NOT NULL,
	"Name"	TEXT NOT NULL COLLATE NOCASE,
	"AccessKey"	TEXT,
	"TotalHoursPlayed" REAL NOT NULL DEFAULT 0.0,
	"CreatedAt"	TEXT NOT NULL,
	CONSTRAINT "PK_Tester" PRIMARY KEY("Id" AUTOINCREMENT)
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_CheckedAt" ON "AccessValidationHistory" (
	"CheckedAt"
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_City" ON "AccessValidationHistory" (
	"City" COLLATE NOCASE
);
CREATE INDEX IF NOT EXISTS "IX_AccessValidationHistory_Country" ON "AccessValidationHistory" (
	"Country" COLLATE NOCASE
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
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Tester_AccessKey" ON "Tester" (
	"AccessKey"
);
CREATE UNIQUE INDEX IF NOT EXISTS "IX_IpGeoCache_IpAddress" ON "IpGeoCache" (
	"IpAddress"
);
COMMIT;
