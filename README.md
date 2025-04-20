# DataMigrationUtilit

В /DataMigrationUtilit.CLI/appsettings.json строка подключения к бд
В /DataMigrationUtilit.CLI/Files лежат файлы с данными таблицы
новые файлы необходимо самим закинуть в эту папку

Есть команда Import fileName(не указываем формат) type
Примеры: 
Import emp сотрдуник
Import job должность
Import dep подразделение

Есть команда Out departmentId(необязательный аргумент)
