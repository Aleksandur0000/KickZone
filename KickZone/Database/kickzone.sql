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
    stock        INT NOT NULL DEFAULT 10,
    created_at   TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (category_id) REFERENCES categories(id)
) ENGINE=InnoDB;

-- ---------------------------------------------
-- Users (customers)
-- ---------------------------------------------
CREATE TABLE IF NOT EXISTS users (
    id         INT AUTO_INCREMENT PRIMARY KEY,
    username   VARCHAR(50)  NOT NULL UNIQUE,
    email      VARCHAR(150) NOT NULL UNIQUE,
    password   VARCHAR(255) NOT NULL,
    first_name VARCHAR(50)  NOT NULL,
    last_name  VARCHAR(50)  NOT NULL,
    role       ENUM('user', 'admin') NOT NULL DEFAULT 'user',
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
    UNIQUE KEY uq_cart_user_shoe (user_id, shoe_id),
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

INSERT INTO shoes (name, description, price, image_url, size, color, brand, category_id, stock) VALUES
    ('Nike Air Max',  'Класически маратонки на Nike',        150.00, '/Images/FY7757_800_800px.jpg', 42, 'Neon Green',   'Nike',   1, 15),
    ('Nike Shox',     'Ретро маратонки Nike',                140.00, '/Images/images.jpg', 42, 'Black/Purple', 'Nike',   1, 10),
    ('Adidas Ultraboost', 'Бъст буст за дълги разстояния', 180.00, '/Images/adidas.jpg',               43, 'White',        'Adidas', 2, 8),
    ('Puma Future',   'Футболни маратонки Puma',             90.00,  '/Images/cherni-sportni-olekoteni-obuvki.jpg',               41, 'Red',          'Puma',   3, 20);

-- =============================================
-- АКО ВЕЧЕ СИ ИМПОРТИРАЛ СТАРАТА ВЕРСИЯ,
-- пусни тези заявки в phpMyAdmin (SQL таб):
--
-- UPDATE shoes SET image_url = '/Images/FY7757_800_800px.jpg' WHERE name = 'Nike Air Max';
-- UPDATE shoes SET image_url = '/Images/images.jpg' WHERE name = 'Nike Shox';
-- UPDATE shoes SET image_url = '/Images/adidas.jpg' WHERE name = 'Adidas Ultraboost';
-- UPDATE shoes SET image_url = '/Images/cherni-sportni-olekoteni-obuvki.jpg' WHERE name = 'Puma Future';
--
-- ALTER TABLE users ADD COLUMN username VARCHAR(50) NOT NULL UNIQUE AFTER id;
-- ALTER TABLE users ADD COLUMN role ENUM('user','admin') NOT NULL DEFAULT 'user' AFTER last_name;
-- ALTER TABLE shoes ADD COLUMN stock INT NOT NULL DEFAULT 10;
-- ALTER TABLE cart_items ADD UNIQUE KEY uq_cart_user_shoe (user_id, shoe_id);
-- =============================================
-- Админът (admin / admin123) се създава
-- автоматично при стартиране на приложението.
-- =============================================
