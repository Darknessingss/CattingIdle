using UnityEngine;

public static class RarityLocalizer
{
    private static string currentLanguage = "en";
    public static event System.Action OnLanguageChanged;

    public static void SetLanguage(string lang)
    {
        currentLanguage = lang;
        OnLanguageChanged?.Invoke();
    }

    public static string GetLocalizedRarity(string rarityName)
    {
        if (currentLanguage == "ru")
        {
            switch (rarityName)
            {
                case "Common": return "Обычная";
                case "Uncommon": return "Необычная";
                case "Rare": return "Редкая";
                case "Epic": return "Эпическая";
                case "Legendary": return "Легендарная";
                case "Nightmare": return "Кошмарная";
                case "Golden": return "Золотая";
                default: return rarityName;
            }
        }
        return rarityName;
    }

    public static string GetLocalizedAnimalName(string animalName)
    {
        if (currentLanguage == "ru")
        {
            switch (animalName)
            {
                case "Lion": return "Лев";
                case "Tiger": return "Тигр";
                case "Bear": return "Медведь";
                case "Wolf": return "Волк";
                case "Fox": return "Лиса";
                case "Rabbit": return "Заяц";
                case "Elephant": return "Слон";
                case "Giraffe": return "Жираф";
                case "Zebra": return "Зебра";
                case "Deer": return "Олень";
                default: return animalName;
            }
        }
        return animalName;
    }

    public static string GetLocalizedPerMin()
    {
        return currentLanguage == "ru" ? "/мин" : "/min";
    }

    public static string GetLocalizedPerSec()
    {
        return currentLanguage == "ru" ? "/сек" : "/sec";
    }

    public static string GetLocalizedFoodNeeded()
    {
        return currentLanguage == "ru" ? "Нужно еды:" : "Food needed:";
    }

    public static string GetLocalizedTamingStatus(bool isTamed, bool canTame, bool isTaming, int progressPercent)
    {
        if (currentLanguage == "ru")
        {
            if (isTamed) return "Приручен";
            if (!canTame) return "ЛИМИТ ЖИВОТНЫХ ПРЕВЫШЕН!";
            if (!isTaming) return "Нажми E для приручения";
            return $"Приручение: {progressPercent}%";
        }
        else
        {
            if (isTamed) return "Tamed";
            if (!canTame) return "ANIMAL LIMIT REACHED!";
            if (!isTaming) return "Press E to tame";
            return $"Taming: {progressPercent}%";
        }
    }
    public static string GetLocalizedCraft()
    {
        return currentLanguage == "ru" ? "Изготовить:" : "Craft:";
    }

    public static string GetLocalizedCost()
    {
        return currentLanguage == "ru" ? "Цена:" : "Cost:";
    }
}