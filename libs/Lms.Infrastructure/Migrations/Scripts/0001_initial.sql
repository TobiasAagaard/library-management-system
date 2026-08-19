CREATE TABLE library (
    id         INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name       VARCHAR(255) NOT NULL,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT library_name_unique UNIQUE (name)
);

CREATE TABLE author (
    id   INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL,

    CONSTRAINT author_name_unique UNIQUE (name)
);

CREATE TABLE book (
    id    INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    isbn  VARCHAR(13)  NOT NULL,

    CONSTRAINT book_isbn_unique UNIQUE (isbn),

    CONSTRAINT book_isbn_format CHECK (isbn ~ '^([0-9]{9}[0-9X]|[0-9]{13})$')
);

CREATE TABLE author (
    book_id   INT      NOT NULL,
    author_id INT      NOT NULL,
    position  SMALLINT NOT NULL DEFAULT 1,

    PRIMARY KEY (book_id, author_id),
    FOREIGN KEY (book_id)   REFERENCES book(id)   ON DELETE CASCADE,
    FOREIGN KEY (author_id) REFERENCES author(id) ON DELETE RESTRICT
);

CREATE TABLE book_copy (
    id         INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    book_id    INT         NOT NULL,
    library_id INT         NOT NULL,
    added_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
    barcode    VARCHAR(20) NOT NULL,

    CONSTRAINT book_copy_barcode_unique UNIQUE (barcode),
    FOREIGN KEY (book_id)    REFERENCES book(id)    ON DELETE RESTRICT,
    FOREIGN KEY (library_id) REFERENCES library(id) ON DELETE CASCADE
);

CREATE TABLE app_user (
    id         INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name  VARCHAR(100) NOT NULL,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT now()
);

CREATE TABLE member (
    library_id INT         NOT NULL,
    user_id    INT         NOT NULL,
    joined_at  TIMESTAMPTZ NOT NULL DEFAULT now(),

    PRIMARY KEY (library_id, user_id),
    FOREIGN KEY (library_id) REFERENCES library(id)  ON DELETE CASCADE,
    FOREIGN KEY (user_id)    REFERENCES app_user(id) ON DELETE CASCADE
);

CREATE TABLE loan (
    id           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    book_copy_id INT         NOT NULL,
    user_id      INT         NOT NULL,
    loaned_at    TIMESTAMPTZ NOT NULL DEFAULT now(),
    due_at       TIMESTAMPTZ NOT NULL DEFAULT (now() + INTERVAL '30 days'),
    returned_at  TIMESTAMPTZ,

    FOREIGN KEY (book_copy_id) REFERENCES book_copy(id) ON DELETE RESTRICT,
    FOREIGN KEY (user_id)      REFERENCES app_user(id)  ON DELETE RESTRICT,

    CONSTRAINT loan_due_after_loaned      CHECK (due_at > loaned_at),
    CONSTRAINT loan_returned_after_loaned CHECK (returned_at IS NULL OR returned_at >= loaned_at)
);