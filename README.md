# goreo
A mountain hiking helper webapp for a Uni project.

# database model
The following code is used to generate the PostgreSQL 14 database for the project
It should be called "postgres"

```sql
CREATE TABLE IF NOT EXISTS booklets
(
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY
);

CREATE TABLE IF NOT EXISTS users
(
    id                INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    email             VARCHAR(255) UNIQUE NOT NULL,
    username          VARCHAR(30) UNIQUE  NOT NULL,
    password          VARCHAR(255)        NOT NULL,
    profile_image     VARCHAR(255) UNIQUE,
    first_name        VARCHAR(255)        NOT NULL,
    surname           VARCHAR(255)        NOT NULL,
    city_of_residence VARCHAR(255)        NOT NULL,
    is_leader         BOOLEAN             NOT NULL,
    leader_id_no      VARCHAR(30) UNIQUE,
    is_admin          BOOLEAN             NOT NULL,
    booklet_id        INT UNIQUE REFERENCES booklets (id) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS routes
(
    id      INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    user_id INT NOT NULL REFERENCES users (id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS booklets_routes
(
    booklet_id INT REFERENCES booklets (id) ON UPDATE CASCADE ON DELETE CASCADE,
    route_id   INT REFERENCES routes (id) ON UPDATE CASCADE,
    entry_date TIMESTAMP NOT NULL,
    CONSTRAINT booklets_routes_pkey PRIMARY KEY (booklet_id, route_id)
);

CREATE TABLE IF NOT EXISTS locations
(
    id           INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    name         VARCHAR(255) UNIQUE NOT NULL,
    height       INT                 NOT NULL,
    x_coordinate NUMERIC,
    y_coordinate NUMERIC,
    description  VARCHAR(255),
    image        VARCHAR(255) UNIQUE
);

CREATE TABLE IF NOT EXISTS sections
(
    id             INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    distance       INT     NOT NULL,
    points         INT     NOT NULL,
    approach       INT     NOT NULL,
    is_counted     BOOLEAN NOT NULL,
    description    VARCHAR(255),
    mountain_trail VARCHAR(255),
    location_from  INT     NOT NULL REFERENCES locations (id) ON UPDATE CASCADE,
    location_to    INT     NOT NULL REFERENCES locations (id) ON UPDATE CASCADE,
    route_id       INT     NOT NULL REFERENCES routes (id) ON UPDATE CASCADE ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS mountain_groups
(
    name VARCHAR(255) PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS locations_mountain_groups
(
    location_id         INT REFERENCES locations (id) ON UPDATE CASCADE ON DELETE CASCADE,
    mountain_group_name VARCHAR(255) REFERENCES mountain_groups (name) ON UPDATE CASCADE,
    CONSTRAINT locations_mountain_groups_pkey PRIMARY KEY (location_id, mountain_group_name)
);

CREATE TABLE IF NOT EXISTS badges
(
    id          INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    name        VARCHAR(255) UNIQUE NOT NULL,
    description VARCHAR(255),
    image       VARCHAR(255) UNIQUE
);

CREATE TABLE IF NOT EXISTS booklets_badges
(
    booklet_id INT REFERENCES booklets (id) ON UPDATE CASCADE ON DELETE CASCADE,
    badge_id   INT REFERENCES badges (id) ON UPDATE CASCADE,
    earn_date  TIMESTAMP NOT NULL,
    CONSTRAINT booklets_badges_pkey PRIMARY KEY (booklet_id, badge_id)
);


ALTER TABLE routes
ADD COLUMN name VARCHAR(255) UNIQUE NOT NULL DEFAULT '';

-- drop table booklets_badges;
-- drop table badges;
-- drop TABLE locations_mountain_groups;
-- drop table mountain_groups;
-- drop table sections;
-- drop table locations;
-- drop table booklets_routes;
-- drop table routes;
-- drop table users;
-- drop table booklets;
```