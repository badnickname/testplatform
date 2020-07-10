### Платформа для тестирования
Веб-приложение на базе ASP.NET Core MVC. Позволяет проводить академические/развлекательные тесты.

### Конфигурирование

Используется база данных MySQL. Конфигурация в файле appsettings.json
```
{
  "Settings": {
    "IP": "",
    "User": "",
    "Pass": "",
    "Database": ""
  },
  

  ```
  
Используется SMTP сервер. Конфигурация в файле mailsettings.json
```
{
  "Mail": {
    "Body" : "Для продолжения регистрации пройдите по ссылке: ",
    "From" : " sender @mail.ru",
    "Title" : "Регистрация на сайте",
    "Port" : 25,
    "Host" : "smtp.mail.ru",
    "UserName" : "  ",
    "Password" : "   ",
    "Address" : "  /Home/ConfirmRegister",
    "DisplayName" : "Online-Testing"
  }
}
```

![Пример](https://gitlab.com/eeeeeeeeeeee/testplatform/raw/master/Doc/img0.png)