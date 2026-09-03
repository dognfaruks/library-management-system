-- ============================================
-- Kütüphane Yönetim Sistemi - Database Schema
-- PostgreSQL
-- ============================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ============ USERS & ROLES ============

CREATE TABLE "Users" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Username" VARCHAR(50) NOT NULL UNIQUE,
    "Email" VARCHAR(150) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE "Roles" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE "UserRoles" (
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "RoleId" INT NOT NULL REFERENCES "Roles"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("UserId", "RoleId")
);

-- ============ CATALOG ============

CREATE TABLE "Authors" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "FullName" VARCHAR(150) NOT NULL
);

CREATE TABLE "Categories" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE "Publishers" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Name" VARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE "Books" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "Title" VARCHAR(255) NOT NULL,
    "ISBN" VARCHAR(20) NOT NULL UNIQUE,
    "PublisherId" UUID NOT NULL REFERENCES "Publishers"("Id") ON DELETE RESTRICT,
    "Stock" INT NOT NULL DEFAULT 0 CHECK ("Stock" >= 0),
    "PublishedYear" INT,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE "BookAuthors" (
    "BookId" UUID NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "AuthorId" UUID NOT NULL REFERENCES "Authors"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("BookId", "AuthorId")
);

CREATE TABLE "BookCategories" (
    "BookId" UUID NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "CategoryId" UUID NOT NULL REFERENCES "Categories"("Id") ON DELETE CASCADE,
    PRIMARY KEY ("BookId", "CategoryId")
);

-- ============ BORROWING & RESERVATION ============

CREATE TABLE "Borrowings" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BookId" UUID NOT NULL REFERENCES "Books"("Id") ON DELETE RESTRICT,
    "BorrowedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "DueDate" TIMESTAMP NOT NULL,
    "ReturnedAt" TIMESTAMP NULL,
    "Status" SMALLINT NOT NULL DEFAULT 0 -- 0=Active, 1=Returned, 2=Overdue
);

CREATE TABLE "Reservations" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BookId" UUID NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "ReservedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "QueueOrder" INT NOT NULL,
    "Status" SMALLINT NOT NULL DEFAULT 0 -- 0=Active, 1=Fulfilled, 2=Cancelled
);

CREATE UNIQUE INDEX "UX_Reservations_User_Book_Active"
    ON "Reservations" ("UserId", "BookId")
    WHERE "Status" = 0;

-- ============ REVIEWS ============

CREATE TABLE "Reviews" (
    "Id" UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    "UserId" UUID NOT NULL REFERENCES "Users"("Id") ON DELETE CASCADE,
    "BookId" UUID NOT NULL REFERENCES "Books"("Id") ON DELETE CASCADE,
    "Rating" SMALLINT NOT NULL CHECK ("Rating" BETWEEN 1 AND 5),
    "Comment" TEXT,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    CONSTRAINT "UQ_Reviews_User_Book" UNIQUE ("UserId", "BookId")
);

-- ============ INDEXES ============

CREATE INDEX "IX_Books_Title" ON "Books" ("Title");
CREATE INDEX "IX_Books_PublisherId" ON "Books" ("PublisherId");
CREATE INDEX "IX_Borrowings_UserId" ON "Borrowings" ("UserId");
CREATE INDEX "IX_Borrowings_BookId" ON "Borrowings" ("BookId");
CREATE INDEX "IX_Borrowings_Status" ON "Borrowings" ("Status");
CREATE INDEX "IX_Reservations_BookId" ON "Reservations" ("BookId");
CREATE INDEX "IX_Reservations_Status" ON "Reservations" ("Status");
CREATE INDEX "IX_Reviews_BookId" ON "Reviews" ("BookId");

-- ============ SEED ROLES ============
INSERT INTO "Roles" ("Name") VALUES ('USER'), ('ADMIN');