using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalizationRestorer : EditorWindow
{
    [MenuItem("Tools/Restore Full Localization")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationRestorer>("Restore Full Localization");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Restore Full Localization", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This will restore all localization sections (GagTexts, PatientReactions, etc.)");
        GUILayout.Label("while preserving the updated PatientTexts.");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Restore Russian Localization"))
        {
            RestoreRussianLocalization();
        }
        
        if (GUILayout.Button("Restore English Localization"))
        {
            RestoreEnglishLocalization();
        }
        
        if (GUILayout.Button("Restore Turkish Localization"))
        {
            RestoreTurkishLocalization();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Restore All Localization Files"))
        {
            RestoreAllLocalization();
        }
    }
    
    void RestoreRussianLocalization()
    {
        string ruPath = "Assets/StreamingAssets/Localization/ru.json";
        
        // Создаем полную структуру русского файла
        var ruData = new LocalizationData();
        ruData.LanguageName = "Русский";
        ruData.LanguageCode = "ru";
        
        // GagTexts
        ruData.GagTexts = new GagTexts
        {
            Clownish = new string[]
            {
                "Почему нет контактных цирков? Где можно гладить воздушных гимнастов и кормить акробатов?",
                "Почему клоун выбросил свои часы из окна? Он хотел посмотреть, как летит время!",
                "Опытный клоун по вызову от души повеселит детишек, не разочарует и одиноких мам…"
            },
            Verbal = new string[]
            {
                "Почему, когда я подарил своей девушке средство для ухода, она не уходит?",
                "Львиная доля туристов не вернулась домой из-за инцидента на сафари.",
                "- Как называют человека, который продал свою печень?\n- Обеспеченный."
            },
            Absurdist = new string[]
            {
                "Вчера мой тапок подал заявление на брак с пылесосом.\nСказал: 'Он единственный, кто меня всасывает!'",
                "Почему носки всегда теряются в стиральной машине?\nПотому что они ищут пару в другом измерении!",
                "Мой бутерброд упал маслом вниз.\nЭто был заговор гравитации!"
            },
            Ironic = new string[]
            {
                "- Вам не трудно сделать мне кофе с пенкой?\n- Да раз плюнуть.",
                "Происшествие в Ростове. Киллер чихнул, а полиция до сих пор ломает голову, кто мог заказать продавца шаурмы…",
                "Семейные новости. Толик назвал своего сына – Евро. Надеется, так он будет расти быстрее…"
            },
            VerbalGag = new string[]
            {
                "Почему гриб не ходит в школу? Его ждут, пока *вырастят*!",
                "— Доктор, я чувствую себя собакой!\n— Сколько лет?\n— Три месяца.",
                "Лучшее лекарство — это когда тебе не выписывают счёт!"
            },
            IronicGag = new string[]
            {
                "О, вы точно выздоровеете… прямо как мои шансы на премию.",
                "Смех — лучшее лекарство? Тогда где мой рецепт на 'хохотин'?",
                "Вы здоровы!.. Шучу. Но было бы смешно, да?"
            }
        };
        
        // PatientReactions
        ruData.PatientReactions = new PatientReactions
        {
            Angry = new string[]
            {
                "Дайте мне другого доктора, этот какой-то идиот!",
                "Вы меня оскорбляете!",
                "Я подам на вас в комиссию!"
            },
            Happy = new string[]
            {
                "Доктор, спасибо, я здоров!",
                "Ха-ха! Мне сразу легче!",
                "Вы — гений! Я выздоравливаю!"
            },
            Neutral = new string[]
            {
                "Доктор, что это сейчас было?",
                "Ну... не смешно.",
                "Я не понял, в чём шутка?"
            },
            BossContinue = new string[]
            {
                "Хм... Интересно. Продолжайте."
            },
            BossFail = new string[]
            {
                "Ещё одна такая шутка — и вас уволят!"
            }
        };
        
        // UITexts
        ruData.UITexts = new UITexts
        {
            RewardScreenTitle = "Выберите награду",
            SelectReward = "Выбрать награду",
            LoadingFloor = "Загрузка этажа...",
            ReturningToMenu = "Возврат в меню",
            VictoryMessage = "Победа!",
            StaircaseNotCured = new string[]
            {
                "Нельзя подняться на следующий этаж, пока не вылечены все пациенты. Осталось: {count}",
                "Сначала вылечите всех пациентов на этом этаже. Осталось: {count}",
                "На следующий этаж можно только после излечения всех пациентов. Осталось: {count}"
            },
            StaircaseNotVisited = new string[]
            {
                "Нельзя подняться на следующий этаж, пока не осмотрены все палаты",
                "Сначала посетите все палаты на этом этаже",
                "Осмотрите все палаты перед подъемом на следующий этаж"
            },
            WardDoorPrompt = "Войти в палату",
            WardExitDoorPrompt = "Выйти из палаты",
            WardExitDoorCuredPrompt = "Выйти из палаты (пациент вылечен!)",
            MedicalRecordPrompt = "Прочитать историю болезни",
            InfoDescPrompt = "Получить информацию",
            StaircaseAvailablePrompt = "Подняться по лестнице",
            StaircaseBlockedPrompt = "Подняться по лестнице (недоступно)"
        };
        
        // DialogueTexts
        ruData.DialogueTexts = new DialogueTexts
        {
            DefaultGag = "Э-э... посмейтесь?",
            BatmanIntro = new string[]
            {
                "Добро пожаловать в нашу клинику, доктор! Пациенты ждут вашей помощи.",
                "Помните: смех - лучшее лекарство. Но не все шутки подходят всем пациентам.",
                "Удачи, доктор! Больница в ваших руках."
            },
            BatmanOutro = new string[]
            {
                "Отличная работа, доктор! Вы справились!",
                "Пациенты выздоровели благодаря вам.",
                "Спасибо за вашу помощь!"
            },
            Hints = new string[]
            {
                "Изучите медицинскую карту пациента перед выбором шутки.",
                "Некоторые типы юмора могут быть запрещены для определенных пациентов.",
                "Чем выше уровень шутки, тем больше шанс успеха даже при неправильном типе."
            },
            Tutorial = new string[]
            {
                "Привет, стажёр! Ты - наша последняя надежда в борьбе с грустью!",
                "Пациенты здесь лечатся только одним способом - смехом. Да-да, серьёзно!",
                "Подойди к любой палате, открой карту и найди ключ к юмору этого человека.",
                "Выбери правильный гэг - и пациент выздоровеет! Не угадаешь - станет только хуже.",
                "Ты можешь совершить не более 3-х ошибок на каждом этаже. Ошибешься в 4й раз и ты уволен, салага!",
                "А теперь - вперёд! Больница не вылечит сама себя... хотя, кто его знает?"
            }
        };
        
        // Сохраняем существующие PatientTexts если они есть
        if (File.Exists(ruPath))
        {
            string existingContent = File.ReadAllText(ruPath);
            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                var existingData = JsonUtility.FromJson<LocalizationData>(existingContent);
                if (existingData?.PatientTexts != null)
                {
                    ruData.PatientTexts = existingData.PatientTexts;
                    Debug.Log("Preserved existing PatientTexts from ru.json");
                }
            }
        }
        
        // Сохраняем файл
        string json = JsonUtility.ToJson(ruData, true);
        File.WriteAllText(ruPath, json);
        
        Debug.Log("Restored complete Russian localization file!");
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Russian Localization Restored", 
            "Successfully restored complete Russian localization with all sections!", "OK");
    }
    
    void RestoreEnglishLocalization()
    {
        string enPath = "Assets/StreamingAssets/Localization/en.json";
        
        var enData = new LocalizationData();
        enData.LanguageName = "English";
        enData.LanguageCode = "en";
        
        // GagTexts
        enData.GagTexts = new GagTexts
        {
            Clownish = new string[]
            {
                "Why aren't there contact circuses? Where can you pet aerial gymnasts and feed acrobats?",
                "Why did the clown throw his watch out the window? He wanted to see time fly!",
                "An experienced clown on call will cheer up kids, won't disappoint single moms..."
            },
            Verbal = new string[]
            {
                "Why, when I gave my girlfriend skincare product, she doesn't leave?",
                "The lion's share of tourists didn't return home due to a safari incident.",
                "- What do you call someone who sold their liver?\n- Well-off."
            },
            Absurdist = new string[]
            {
                "Yesterday my slipper filed for marriage with a vacuum cleaner.\nIt said: 'It's the only one that sucks me in!'",
                "Why do socks always get lost in the washing machine?\nBecause they're looking for a pair in another dimension!",
                "My sandwich fell butter-side down.\nIt was a gravity conspiracy!"
            },
            Ironic = new string[]
            {
                "- Could you make me coffee with foam?\n- Easy peasy.",
                "Incident in Rostov. A killer sneezed, and police are still racking their brains over who could have ordered the shawarma seller...",
                "Family news. Tolik named his son Euro. Hopes he'll grow faster..."
            },
            VerbalGag = new string[]
            {
                "Why doesn't a mushroom go to school? They're waiting for it to *grow up*!",
                "— Doctor, I feel like a dog!\n— How old?\n— Three months.",
                "The best medicine is when they don't bill you!"
            },
            IronicGag = new string[]
            {
                "Oh, you'll definitely recover... just like my chances for a bonus.",
                "Laughter is the best medicine? Then where's my prescription for 'laughter'?",
                "You're healthy!.. Just kidding. But it would be funny, wouldn't it?"
            }
        };
        
        // PatientReactions
        enData.PatientReactions = new PatientReactions
        {
            Angry = new string[]
            {
                "Give me another doctor, this one is some kind of idiot!",
                "You're offending me!",
                "I'll report you to the commission!"
            },
            Happy = new string[]
            {
                "Doctor, thank you, I'm healthy!",
                "Ha-ha! I feel better immediately!",
                "You're a genius! I'm recovering!"
            },
            Neutral = new string[]
            {
                "Doctor, what was that just now?",
                "Well... not funny.",
                "I don't get what the joke is?"
            },
            BossContinue = new string[]
            {
                "Hmm... Interesting. Continue."
            },
            BossFail = new string[]
            {
                "One more joke like that and you're fired!"
            }
        };
        
        // UITexts
        enData.UITexts = new UITexts
        {
            RewardScreenTitle = "Choose Reward",
            SelectReward = "Select Reward",
            LoadingFloor = "Loading floor...",
            ReturningToMenu = "Returning to menu",
            VictoryMessage = "Victory!",
            StaircaseNotCured = new string[]
            {
                "Cannot go to the next floor until all patients are cured. Remaining: {count}",
                "First cure all patients on this floor. Remaining: {count}",
                "Next floor is only available after curing all patients. Remaining: {count}"
            },
            StaircaseNotVisited = new string[]
            {
                "Cannot go to the next floor until all wards are visited",
                "First visit all wards on this floor",
                "Visit all wards before going to the next floor"
            },
            WardDoorPrompt = "Enter ward",
            WardExitDoorPrompt = "Exit ward",
            WardExitDoorCuredPrompt = "Exit ward (patient cured!)",
            MedicalRecordPrompt = "Read medical record",
            InfoDescPrompt = "Get information",
            StaircaseAvailablePrompt = "Go up stairs",
            StaircaseBlockedPrompt = "Go up stairs (unavailable)"
        };
        
        // DialogueTexts
        enData.DialogueTexts = new DialogueTexts
        {
            DefaultGag = "Uh... laugh, please?",
            BatmanIntro = new string[]
            {
                "Welcome to our clinic, doctor! Patients are waiting for your help.",
                "Remember: laughter is the best medicine. But not all jokes work for all patients.",
                "Good luck, doctor! The hospital is in your hands."
            },
            BatmanOutro = new string[]
            {
                "Excellent work, doctor! You've succeeded!",
                "Patients have recovered thanks to you.",
                "Thank you for your help!"
            },
            Hints = new string[]
            {
                "Study the patient's medical card before choosing a joke.",
                "Some types of humor may be forbidden for certain patients.",
                "The higher the joke level, the greater the chance of success even with the wrong type."
            },
            Tutorial = new string[]
            {
                "Hello, intern! You are our last hope in fight against sadness!",
                "Patients here are treated in only one way - with laughter. Yes, seriously!",
                "Go to any ward, open card and find the key to this person's humor.",
                "Choose the right gag - and the patient will recover! If you guess wrong - it will only get worse.",
                "You can make no more than 3 mistakes on each floor. Make a 4th mistake and you're fired, rookie!",
                "And now - forward! The hospital won't heal itself... although, who knows?"
            }
        };
        
        // Сохраняем существующие PatientTexts если они есть
        if (File.Exists(enPath))
        {
            string existingContent = File.ReadAllText(enPath);
            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                var existingData = JsonUtility.FromJson<LocalizationData>(existingContent);
                if (existingData?.PatientTexts != null)
                {
                    enData.PatientTexts = existingData.PatientTexts;
                    Debug.Log("Preserved existing PatientTexts from en.json");
                }
            }
        }
        
        string json = JsonUtility.ToJson(enData, true);
        File.WriteAllText(enPath, json);
        
        Debug.Log("Restored complete English localization file!");
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("English Localization Restored", 
            "Successfully restored complete English localization with all sections!", "OK");
    }
    
    void RestoreTurkishLocalization()
    {
        string trPath = "Assets/StreamingAssets/Localization/tr.json";
        
        var trData = new LocalizationData();
        trData.LanguageName = "Türkçe";
        trData.LanguageCode = "tr";
        
        // GagTexts
        trData.GagTexts = new GagTexts
        {
            Clownish = new string[]
            {
                "Neden temas temas sirk yok? Havadaki jimnastiklarla oynayıp akrobatları besleyebilirsiniz?",
                "Palyaço neden saatini pencereden attı? Zamanın nasıl uçtuğunu görmek istedi!",
                "Deneyimli bir palyaço çağrıda çocukları eğlendirir, yalnız anneleri hayal kırmaz..."
            },
            Verbal = new string[]
            {
                "Neden kız arkadaşımı cilt bakım ürünü verdiğimde gitmiyor?",
                "Turistlerin aslan payı safari olayı nedeniyle eve dönmedi.",
                "- Karaciğerini satan kişiye ne denir?\n- Zengin."
            },
            Absurdist = new string[]
            {
                "Dün terliğim süpürge ile evlilik için başvuru yaptı.\n'Beni emen tek o' dedi.",
                "Neden çoraplar her zaman çamaşır makinesinde kaybolur?\nÇünkü başka boyutta arıyorlar!",
                "Sandviçim tereyağı aşağı düştü.\nBu bir yerçekimi komplosuydu!"
            },
            Ironic = new string[]
            {
                "- Bana köpüklü kahve yapması zor olur mu?\n- Kolaydı.",
                "Rostov'da olay. Bir tetikçinin hapşırması, polis hala şavurma satıcısını kim sipariş edebileceğini kafasını yırtıyor...",
                "Aile haberleri. Tolik oğlunun adını Euro koydu. Daha hızlı büyüyeceğini umuyor..."
            },
            VerbalGag = new string[]
            {
                "Neden mantar okula gitmez? Onu *büyüteceklerini* bekliyorlar!",
                "— Doktor, kendimi bir köpek gibi hissediyorum!\n— Kaç yaşındasın?\n— Üç ay.",
                "En iyi ilaç, sana fatura kesmemektir!"
            },
            IronicGag = new string[]
            {
                "Oh, kesinlikle iyileşeceksin... tıpkı prim şanslarım gibi.",
                "Gülme en iyi ilaçsa, 'kahkaha' reçetem nerede?",
                "Sağlısın!.. Şaka yapıyorum. Ama komik olurdu, değil mi?"
            }
        };
        
        // PatientReactions
        trData.PatientReactions = new PatientReactions
        {
            Angry = new string[]
            {
                "Bana başka bir doktor verin, bu bir çeşit aptal!",
                "Beni incitiyorsunuz!",
                "Sizi komisyona şikayet edeceğim!"
            },
            Happy = new string[]
            {
                "Doktor, teşekkürler, sağlıyım!",
                "Ha-ha! Hemen daha iyi hissediyorum!",
                "Bir dahi'siniz! İyileşiyorum!"
            },
            Neutral = new string[]
            {
                "Doktor, bu şimdi neydi?",
                "Eh... komik değil.",
                "Şakanın ne olduğunu anlamadım?"
            },
            BossContinue = new string[]
            {
                "Hmm... İlginç. Devam edin."
            },
            BossFail = new string[]
            {
                "Böyle bir şaka daha olursa ve kovulursunuz!"
            }
        };
        
        // UITexts
        trData.UITexts = new UITexts
        {
            RewardScreenTitle = "Ödül Seç",
            SelectReward = "Ödül Seç",
            LoadingFloor = "Kat yükleniyor...",
            ReturningToMenu = "Menüye dön",
            VictoryMessage = "Zafer!",
            StaircaseNotCured = new string[]
            {
                "Tüm hastalar iyileşene kadar bir sonraki kata çıkılamaz. Kalan: {count}",
                "Önce bu kattaki tüm hastaları iyileştirin. Kalan: {count}",
                "Bir sonraki kat sadece tüm hastalar iyileştirildikten sonra kullanılabilir. Kalan: {count}"
            },
            StaircaseNotVisited = new string[]
            {
                "Tüm odalar ziyaret edilene kadar bir sonraki kata çıkılamaz",
                "Önce bu kattaki tüm odaları ziyaret edin",
                "Bir sonraki kata çıkmadan önce tüm odaları ziyaret edin"
            },
            WardDoorPrompt = "Odaya gir",
            WardExitDoorPrompt = "Odadan çık",
            WardExitDoorCuredPrompt = "Odadan çık (hasta iyileşti!)",
            MedicalRecordPrompt = "Tıbbi kaydı oku",
            InfoDescPrompt = "Bilgi al",
            StaircaseAvailablePrompt = "Merdiven çık",
            StaircaseBlockedPrompt = "Merdiven çık (mevcut değil)"
        };
        
        // DialogueTexts
        trData.DialogueTexts = new DialogueTexts
        {
            DefaultGag = "Ee... gülsenize?",
            BatmanIntro = new string[]
            {
                "Kliniğimize hoş geldiniz, doktor! Hastalar yardımınızı bekliyor.",
                "Unutmayın: gülme en iyi ilaçtır. Ama tüm şakalar tüm hastalara uymaz.",
                "İyi şanslar, doktor! Hastane sizin ellerinizde."
            },
            BatmanOutro = new string[]
            {
                "Mükemmel iş, doktor! Başardınız!",
                "Hastalar sizin sayenizde iyileşti.",
                "Yardımınız için teşekkürler!"
            },
            Hints = new string[]
            {
                "Şaka seçmeden önce hastanın tıbbi kartını inceleyin.",
                "Bazı mizah türleri belirli hastalar için yasaklanabilir.",
                "Şaka seviyesi ne kadar yüksekse, yanlış türde bile başarı şansı o kadar artar."
            },
            Tutorial = new string[]
            {
                "Merhaba, stajyer! Üzüntüyle mücadelede son umudumuzsun!",
                "Buradaki hastalar sadece bir şekilde tedavi edilir - kahkaha ile. Evet, ciddiyim!",
                "Herhangi bir odaya git, kartı aç ve bu kişinin mizah anahtarını bul.",
                "Doğru şakayı seç - ve hasta iyileşecek! Yanlış tahmin edersen - sadece daha da kötüleşir.",
                "Her katta en fazla 3 hata yapabilirsin. 4. hatayı yaparsan kovulursun, acemi!",
                "Ve şimdi - ileri! Hastane kendini iyileştiremez... ama kim bilir?"
            }
        };
        
        // Сохраняем существующие PatientTexts если они есть
        if (File.Exists(trPath))
        {
            string existingContent = File.ReadAllText(trPath);
            if (!string.IsNullOrWhiteSpace(existingContent))
            {
                var existingData = JsonUtility.FromJson<LocalizationData>(existingContent);
                if (existingData?.PatientTexts != null)
                {
                    trData.PatientTexts = existingData.PatientTexts;
                    Debug.Log("Preserved existing PatientTexts from tr.json");
                }
            }
        }
        
        string json = JsonUtility.ToJson(trData, true);
        File.WriteAllText(trPath, json);
        
        Debug.Log("Restored complete Turkish localization file!");
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Turkish Localization Restored", 
            "Successfully restored complete Turkish localization with all sections!", "OK");
    }
    
    void RestoreAllLocalization()
    {
        RestoreRussianLocalization();
        RestoreEnglishLocalization();
        RestoreTurkishLocalization();
        
        EditorUtility.DisplayDialog("All Localization Restored", 
            "Successfully restored all localization files with complete sections!", "OK");
    }
    
    // Классы для структуры данных (такие же как в LocalizationRebuilder)
    [System.Serializable]
    public class LocalizationData
    {
        public string LanguageName;
        public string LanguageCode;
        public GagTexts GagTexts;
        public PatientReactions PatientReactions;
        public UITexts UITexts;
        public DialogueTexts DialogueTexts;
        public PatientTexts PatientTexts;
    }
    
    [System.Serializable]
    public class GagTexts
    {
        public string[] Clownish;
        public string[] Verbal;
        public string[] Absurdist;
        public string[] Ironic;
        public string[] VerbalGag;
        public string[] IronicGag;
    }
    
    [System.Serializable]
    public class PatientReactions
    {
        public string[] Angry;
        public string[] Happy;
        public string[] Neutral;
        public string[] BossContinue;
        public string[] BossFail;
    }
    
    [System.Serializable]
    public class UITexts
    {
        public string RewardScreenTitle;
        public string SelectReward;
        public string LoadingFloor;
        public string ReturningToMenu;
        public string VictoryMessage;
        public string[] StaircaseNotCured;
        public string[] StaircaseNotVisited;
        public string WardDoorPrompt;
        public string WardExitDoorPrompt;
        public string WardExitDoorCuredPrompt;
        public string MedicalRecordPrompt;
        public string InfoDescPrompt;
        public string StaircaseAvailablePrompt;
        public string StaircaseBlockedPrompt;
    }
    
    [System.Serializable]
    public class DialogueTexts
    {
        public string DefaultGag;
        public string[] BatmanIntro;
        public string[] BatmanOutro;
        public string[] Hints;
        public string[] Tutorial;
    }
    
    [System.Serializable]
    public class PatientTexts
    {
        public string[] PatientNames;
        public string[] Diagnoses;
        public string[] Anamnesis;
    }
}
