CREATE DATABASE social_network_db;

USE social_network_db;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    Login VARCHAR(255) UNIQUE NOT NULL,
    `password` VARCHAR(255) NOT NULL,
    birthdate VARCHAR(255) NOT NULL,
    online TINYINT(1) NOT NULL DEFAULT 0,
    ip VARCHAR(255)
);

CREATE TABLE chat_history (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_first INT,
    id_second INT,
    TextLines TEXT NOT NULL,
    FOREIGN KEY (id_first) REFERENCES users(id),
    FOREIGN KEY (id_second) REFERENCES users(id),
    CHECK (id_first != id_second)
);

CREATE TABLE `group`(
    id INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(255) UNIQUE,
    id_group_owner INT,
    FOREIGN KEY (id_group_owner) REFERENCES users(id)
    ON DELETE CASCADE
);

CREATE TABLE groups_users(
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_group INT,
    id_member INT,
    FOREIGN KEY (id_group) REFERENCES `group`(id)
    ON DELETE CASCADE,
    FOREIGN KEY (id_member) REFERENCES users(id)
);

CREATE TABLE group_history(
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_group INT,
    TextLines TEXT NOT NULL,
    FOREIGN KEY (id_group) REFERENCES `group`(id)
    ON DELETE CASCADE
);

DELIMITER $$
CREATE TRIGGER after_group_insert
AFTER INSERT ON `group`
FOR EACH ROW
BEGIN
    INSERT INTO groups_users (id_group, id_member)
    VALUES (NEW.id, NEW.id_group_owner);
END $$
DELIMITER $$;

DELIMITER $$
CREATE TRIGGER prevent_reverse_duplicate_insert
BEFORE INSERT ON chat_history
FOR EACH ROW
BEGIN
    DECLARE reverse_exists INT;

    SELECT COUNT(*) INTO reverse_exists
    FROM chat_history
    WHERE (id_first = NEW.id_second AND id_second = NEW.id_first)
    OR (id_first = NEW.id_first AND id_second = NEW.id_second);

    IF reverse_exists > 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Duplicate connection in reverse order is not allowed';
    END IF;
END$$
DELIMITER $$;


DELIMITER $$
CREATE TRIGGER after_group_insert2
AFTER INSERT ON `group`
FOR EACH ROW
BEGIN
    INSERT INTO groups_users (id_group, id_member)
    VALUES (NEW.id, NEW.id_group_owner);

    INSERT INTO group_history (id_group, TextLines)
    VALUES (NEW.id, '');
END $$
DELIMITER $$;




#ALTER TABLE users CHANGE username Login VARCHAR(255) NOT NULL; 

#SHOW COLUMNS FROM users;












