CREATE TABLE IF NOT EXISTS words (
    id UUID PRIMARY KEY,
    text VARCHAR(100) NOT NULL,
    meaning TEXT NOT NULL,
    synonyms JSONB NOT NULL,
    usage_example TEXT NOT NULL,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP NULL
);

CREATE INDEX IF NOT EXISTS idx_words_text ON words(text);
