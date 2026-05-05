CREATE TABLE _content_buildings
(
    id            TEXT PRIMARY KEY,
    "title"       VARCHAR,
    "body"        TEXT,
    "categories"  TEXT,
    "desc"        VARCHAR,
    "description" VARCHAR,
    "dynasties"   TEXT,
    "extension"   VARCHAR,
    "img"         VARCHAR,
    "meta"        TEXT,
    "name"        VARCHAR,
    "navigation"  TEXT DEFAULT true,
    "path"        VARCHAR,
    "provinces"   TEXT,
    "seo"         TEXT DEFAULT '{}',
    "stem"        VARCHAR,
    "subtitle"    VARCHAR,
    "__hash__"    TEXT UNIQUE
)
