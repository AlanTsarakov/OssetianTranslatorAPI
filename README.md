# Осетинский переводчик (API)

ASP.NET Core backend для распознавания и перевода осетинского текста с фотографий. Принимает изображение, сжимает его и отправляет в OpenAI, возвращая перевод.

## Функциональность

- `POST /api/translate-image` — загрузка фото (multipart), возвращает JSON:
  ```json
  { "success": true, "translation": "{\"original\":\"...\",\"translation\":\"...\"}", "error": null }
  ```
- Сжатие изображения до 1024px через SixLabors.ImageSharp
- Вызов OpenAI (vision) с моделью из конфигурации
- CORS для веб-клиента

## Стек

- ASP.NET Core 10 / .NET 10
- SixLabors.ImageSharp 4.x (нужна лицензия, файл `sixlabors.lic`)
- OpenAI API

## Конфигурация

Настройки в `appsettings.json`, секция `OpenAI`:

```json
"OpenAI": {
  "ApiKey": "sk-...",          // лучше через переменную окружения OpenAI__ApiKey
  "Model": "gpt-5.6-luna",
  "Endpoint": "https://api.openai.com/v1/chat/completions",
  "TimeoutSeconds": 180,
  "MaxImageWidth": 1024
}
```

## Запуск (локально)

```bash
dotnet run
# по умолчанию http://localhost:5098 (см. launchSettings.json)
```

## Публикация под Linux

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
# результат в bin/Release/net10.0/linux-x64/publish/
```

На сервере запускается через systemd как сервис (`ASPNETCORE_URLS=http://127.0.0.1:5098`, ключ через `OpenAI__ApiKey`).
