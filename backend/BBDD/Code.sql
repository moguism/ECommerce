CREATE TABLE users
(
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  name VARCHAR(45) NOT NULL,
  email VARCHAR(255) UNIQUE NOT NULL,
  password VARCHAR(255) NOT NULL,
  role VARCHAR(45) NOT NULL,
  address VARCHAR(255) NOT NULL
);

CREATE TABLE orders
(
  created_at DATE NOT NULL,
  is_reserved INT NOT NULL,
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  user_id INT NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(id)
);

CREATE TABLE payments_types
(
  name VARCHAR(45) NOT NULL,
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL
);

CREATE TABLE categories
(
  name VARCHAR(45) UNIQUE NOT NULL,
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE products
(
  name VARCHAR(45) NOT NULL,
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  description VARCHAR(255) NOT NULL,
  price FLOAT NOT NULL,
  stock INT NOT NULL,
  average FLOAT NOT NULL,
  category_id INT NOT NULL,
  FOREIGN KEY (category_id) REFERENCES categories(id),
  PRIMARY KEY (id)
);

CREATE TABLE orders_products
(
  product_id INT NOT NULL,
  order_id INT NOT NULL,
  PRIMARY KEY (product_id, order_id),
  FOREIGN KEY (product_id) REFERENCES orders(id),
  FOREIGN KEY (order_id) REFERENCES products(id)
);

CREATE TABLE payments
(
  created_at DATE NOT NULL,
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  total FLOAT NOT NULL,
  is_done INT NOT NULL,
  order_id INT NOT NULL,
  payment_type_id INT NOT NULL,
  FOREIGN KEY (order_id) REFERENCES orders(id),
  FOREIGN KEY (payment_type_id) REFERENCES payments_types(id)
);

CREATE TABLE reviews
(
  id INT PRIMARY KEY AUTOINCREMENT NOT NULL,
  text VARCHAR(255) NOT NULL,
  score INT NOT NULL,
  user_id INT NOT NULL,
  product_id INT NOT NULL,
  FOREIGN KEY (user_id) REFERENCES users(id),
  FOREIGN KEY (product_id) REFERENCES products(id)
);