-- Cria os bancos
CREATE DATABASE IF NOT EXISTS EstoqueDB;
CREATE DATABASE IF NOT EXISTS VendasDB;

-- Cria ou garante privilégios do usuário dev
CREATE USER IF NOT EXISTS 'dev'@'%' IDENTIFIED BY 'T0245i@';
GRANT ALL PRIVILEGES ON EstoqueDB.* TO 'dev'@'%';
GRANT ALL PRIVILEGES ON VendasDB.* TO 'dev'@'%';
FLUSH PRIVILEGES;
