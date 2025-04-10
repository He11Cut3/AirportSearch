# 🛫AirportSearch C# Project - Полное руководство

## 🛠 Технический стек
- Язык: C# 10
- Платформа: .NET 8.0
- Дополнительно: MemoryMappedFiles для работы с большими файлами

## ❌ Почему Maven НЕ ПОДХОДИТ
1. **Языковая несовместимость**  
   Maven создан исключительно для Java-проектов:
   - Не понимает C#-синтаксис
   - Не может компилировать `.cs` файлы
   - Не поддерживает NuGet-пакеты

2. **Архитектурные различия**  
   | Компонент       | Java (Maven)          | C# (.NET)            |
   |----------------|----------------------|---------------------|
   | Байт-код       | JVM                  | CLR                 |
   | Менеджер пакетов | Maven Central        | NuGet               |
   | Исполняемый формат | .jar              | .dll/.exe           |

## 🚀 Полная инструкция по запуску

### 1. Подготовка окружения
**Требования:**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- 500 MB свободного места (для больших CSV)

```powershell
# Проверка установки
dotnet --version
```
### 2. Настройка проекта

**Структура файлов:** 

```markdown
📦 airport-search
├── 📦 AirportFiles
├── 📦 bin
├── 📦 obj
└── 📄 airports.dat  ⚠️ Обязательный файл
```

**Требования к файлу данных** 
- Формат: CSV с разделителями-запятыми
- Кодировка: UTF-8
- Минимальный размер: 1MB (для тестирования производительности)

### 3. Запуск приложения

**Откройте терминал (консоль)**
- **Windows**: `Win + R` → введите `cmd` → `Enter`  

**Перейдите в папку проекта**
```bash
cd путь/к/вашей/папке/с/проектом
```

```powershell
dotnet run -- <номер_колонки>
```
**Примеры** 
```powershell
# Поиск по 2-й колонке (название аэропорта)
dotnet run -- 2
Введите текст для поиска (или 'quit' для выхода):
"название аэропорта"

# Поиск по 5-й колонке (код IATA)
dotnet run -- 5
Введите текст для поиска (или 'quit' для выхода):
"код IATA"

# Вывод всех записей по 3-й колонке (город)
dotnet run -- 3
Введите текст для поиска (или 'quit' для выхода):
"город"
```


