# ER Diyagramı

```mermaid
erDiagram
    USERS ||--o{ USERROLES : has
    ROLES ||--o{ USERROLES : has
    USERS ||--o{ BORROWINGS : makes
    USERS ||--o{ RESERVATIONS : makes
    USERS ||--o{ REVIEWS : writes
    BOOKS ||--o{ BORROWINGS : "is borrowed"
    BOOKS ||--o{ RESERVATIONS : "is reserved"
    BOOKS ||--o{ REVIEWS : receives
    BOOKS ||--o{ BOOKAUTHORS : has
    AUTHORS ||--o{ BOOKAUTHORS : writes
    BOOKS ||--o{ BOOKCATEGORIES : has
    CATEGORIES ||--o{ BOOKCATEGORIES : has
    PUBLISHERS ||--o{ BOOKS : publishes

    USERS {
        uuid Id PK
        string Username UK
        string Email UK
        string PasswordHash
        datetime CreatedAt
    }
    ROLES {
        int Id PK
        string Name UK
    }
    USERROLES {
        uuid UserId PK,FK
        int RoleId PK,FK
    }
    AUTHORS {
        uuid Id PK
        string FullName
    }
    CATEGORIES {
        uuid Id PK
        string Name UK
    }
    PUBLISHERS {
        uuid Id PK
        string Name UK
    }
    BOOKS {
        uuid Id PK
        string Title
        string ISBN UK
        uuid PublisherId FK
        int Stock
        int PublishedYear
        string Description
    }
    BOOKAUTHORS {
        uuid BookId PK,FK
        uuid AuthorId PK,FK
    }
    BOOKCATEGORIES {
        uuid BookId PK,FK
        uuid CategoryId PK,FK
    }
    BORROWINGS {
        uuid Id PK
        uuid UserId FK
        uuid BookId FK
        datetime BorrowedAt
        datetime DueDate
        datetime ReturnedAt
        int Status
    }
    RESERVATIONS {
        uuid Id PK
        uuid UserId FK
        uuid BookId FK
        datetime ReservedAt
        int QueueOrder
        int Status
    }
    REVIEWS {
        uuid Id PK
        uuid UserId FK
        uuid BookId FK
        int Rating
        string Comment
        datetime CreatedAt
    }
```

## Composite Key Kullanılan Tablolar
- **UserRoles**: (UserId, RoleId)
- **BookAuthors**: (BookId, AuthorId)
- **BookCategories**: (BookId, CategoryId)

## Unique Alanlar
- Users.Username, Users.Email
- Roles.Name
- Categories.Name, Publishers.Name
- Books.ISBN
- Reviews: (UserId, BookId) composite unique — bir kullanıcı bir kitaba tek yorum
- Reservations: (UserId, BookId) composite unique **where Status = Active** (partial unique index)

## Index Gerektiren Alanlar
- Books.Title (arama için)
- Books.ISBN
- Borrowings.UserId, Borrowings.BookId, Borrowings.Status
- Reservations.BookId, Reservations.Status
- Reviews.BookId
- Tüm FK alanları