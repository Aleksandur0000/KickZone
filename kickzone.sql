-- =============================================
--  KickZone - MariaDB / XAMPP Database
-- =============================================

CREATE DATABASE IF NOT EXISTS kickzone
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE kickzone;

-- ---------------------------------------------
-- Categories
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS categories (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    description TEXT NULL,
    created_at  TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Shoes
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS shoes (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    name         VARCHAR(150) NOT NULL,
    description  TEXT NULL,
    price        DECIMAL(10,2) NOT NULL CHECK (price >= 0),
    image_url    VARCHAR(255) NULL,
    size         INT NOT NULL,
    color        VARCHAR(50)  NOT NULL,
    brand        VARCHAR(100) NOT NULL,
    category_id  INT NOT NULL,
    created_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES categories(id)
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Users (customers)
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    email      VARCHAR(150) NOT NULL UNIQUE,
    password   VARCHAR(255) NOT NULL,
    first_name VARCHAR(50)  NOT NULL,
    last_name  VARCHAR(50)  NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Cart items
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS cart_items (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    user_id    INT NOT NULL,
    shoe_id    INT NOT NULL,
    quantity   INT NOT NULL DEFAULT 1 CHECK (quantity > 0),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (shoe_id) REFERENCES shoes(id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Orders
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS orders (
    id           INT AUTO_INCREMENT PRIMARY KEY,
    user_id      INT NOT NULL,
    total_price  DECIMAL(10,2) NOT NULL DEFAULT 0,
    status       ENUM('pending', 'paid', 'shipped', 'delivered', 'cancelled')
                 NOT NULL DEFAULT 'pending',
    created_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id)
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Order items
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS order_items (
    id          INT AUTO_INCREMENT PRIMARY KEY,
    order_id    INT NOT NULL,
    shoe_id     INT NOT NULL,
    quantity    INT NOT NULL CHECK (quantity > 0),
    unit_price  DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (order_id) REFERENCES orders(id) ON DELETE CASCADE,
    FOREIGN KEY (shoe_id)  REFERENCES shoes(id)
) ENGINE=InnoDB;

-- =============================================
-- Sample data
-- =============================================
INSERT INTO categories (name, description) VALUES
    ('Sneakers', 'Улични маратонки'),
    ('Running',  'Маратонки за бягане'),
    ('Football', 'Футболни обувки');

INSERT INTO shoes (name, description, price, image_url, size, color, brand, category_id) VALUES
    ('Nike Air Max',  'Класически маратонки на Nike',        150.00, 'Images/11111.jpg', 42, 'Neon Green',   'Nike',   1),
    ('Nike Shox',     'Ретро маратонки Nike',                140.00, 'Images/11111.jpg', 42, 'Black/Purple', 'Nike',   1),
    ('Adidas Ultraboost', 'Бъст буст за дълги разстояния', 180.00, NULL,               43, 'White',        'Adidas', 2),
    ('Puma Future',   'Футболни маратонки Puma',             90.00,  NULL,               41, 'Red',          'Puma',   3);
