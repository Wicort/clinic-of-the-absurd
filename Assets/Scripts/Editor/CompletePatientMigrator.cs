using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CompletePatientMigrator : EditorWindow
{
    private Dictionary<string, string> nameToKey = new Dictionary<string, string>();
    private Dictionary<string, string> diagnosisToKey = new Dictionary<string, string>();
    private Dictionary<string, string> anamnesisToKey = new Dictionary<string, string>();
    
    [MenuItem("Tools/Complete Patient Migration")]
    public static void ShowWindow()
    {
        GetWindow<CompletePatientMigrator>("Complete Patient Migration");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Complete Patient Migration to Localization", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Initialize All Key Mappings"))
        {
            InitializeAllKeyMappings();
        }
        
        if (GUILayout.Button("Update All Patient Profiles"))
        {
            UpdateAllProfiles();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will update ALL patient profiles with proper localization keys");
        GUILayout.Label("based on their actual content from the asset files.");
    }
    
    void InitializeAllKeyMappings()
    {
        // Имена пациентов
        nameToKey.Add("Анна Семёновна", "patient_anna_semenova");
        nameToKey.Add("Витя-Прыгун", "patient_vitya_prygun");
        nameToKey.Add("Лёва Облачко", "patient_lev_oblachko");
        nameToKey.Add("Дядя Миша", "patient_uncle_misha");
        nameToKey.Add("Соня-Нос", "patient_sonya_nos");
        nameToKey.Add("Аркадий Скептик", "patient_arkady_skeptic");
        nameToKey.Add("Зинаида-Космос", "patient_zinaida_cosmos");
        nameToKey.Add("Профессор Буквоед", "patient_professor_bukvoed");
        nameToKey.Add("Вера Реалист", "patient_vera_realist");
        nameToKey.Add("Борис-Кувырок", "patient_boris_kuvyrok");
        nameToKey.Add("Главврач Грустин", "patient_chief_doctor_grustin");
        
        // Диагнозы
        diagnosisToKey.Add("Недостаток каламбуров", "diagnosis_pun_deficiency");
        diagnosisToKey.Add("Подавленная потребность в падениях", "diagnosis_fall_suppression");
        diagnosisToKey.Add("Синдром говорящего чайника", "diagnosis_talking_teapot");
        diagnosisToKey.Add("Острая нехватка \"мурзилок\"", "diagnosis_murzilka_deficiency");
        diagnosisToKey.Add("Застойный весёлый рожиц", "diagnosis_stagnant_faces");
        diagnosisToKey.Add("Циничная апатия", "diagnosis_cynical_apathy");
        diagnosisToKey.Add("Вербальный контакт с луной", "diagnosis_moon_contact");
        diagnosisToKey.Add("Гипертрофия каламбуров", "diagnosis_pun_hypertrophy");
        diagnosisToKey.Add("Пессимистический прагматизм", "diagnosis_pessimistic_pragmatism");
        diagnosisToKey.Add("Подавленный рефлекс смеха при падении", "diagnosis_suppressed_laughter_reflex");
        diagnosisToKey.Add("Абсолютная аура серьёзности (иммунитет к одиночным гэгам)", "diagnosis_absolute_aura");
        
        // Анамнезы Анны Семёновны
        anamnesisToKey.Add("Постоянно спрашивает: \"А вы слыхали анекдот про тёщу и холодильник?\"", "anamnesis_anna_jokes");
        anamnesisToKey.Add("Вчера смеялась над надписью \"Туалет\" на двери столовой.", "anamnesis_anna_toilet_sign");
        anamnesisToKey.Add("Пыталась придумать, как сказать \"хлеб\" на языке инопланетян.", "anamnesis_anna_alien_bread");
        
        // Анамнезы Витя-Прыгун
        anamnesisToKey.Add("Улыбнулся, когда медсестра уронила поднос с тарелками.", "anamnesis_vitya_tray_fall");
        anamnesisToKey.Add("Пытался повторить падение с табурета, но упал неудачно.", "anamnesis_vitya_stool_fall");
        anamnesisToKey.Add("Смеётся, когда кто-то чихает громко.", "anamnesis_vitya_sneeze");
        
        // Анамнезы Лёва Облачко
        anamnesisToKey.Add("Утверждает, что его тапок - шпион из будущего.", "anamnesis_lev_spy_slipper");
        anamnesisToKey.Add("Пытался накормить цветок супом из телевизора.", "anamnesis_lev_tv_soup");
        anamnesisToKey.Add("Говорит, что облака — это вата, которую забыли убирать боги.", "anamnesis_lev_cloud_cotton");
        
        // Анамнезы Дядя Миша
        anamnesisToKey.Add("Цитирует рекламу 90-х: \"Пельмешки — не жизнь, а песня!\"", "anamnesis_misha_90s_ad");
        anamnesisToKey.Add("Пытался придумать, как звучит слово \"радуга\" наоборот.", "anamnesis_misha_rainbow_backwards");
        anamnesisToKey.Add("Смеётся, когда называешь ложку \"вилкой для супа\".", "anamnesis_misha_spoon_fork");
        
        // Анамнезы Соня-Нос
        anamnesisToKey.Add("Делает гримасы, глядя в зеркало умывальника.", "anamnesis_sonya_mirrors");
        anamnesisToKey.Add("Пыталась изобразить клоуна с помощью ватных палочек.", "anamnesis_sonya_cotton_clown");
        anamnesisToKey.Add("Смеётся, когда кто-то наталкивается на двенадцатый косяк.", "anamnesis_sonya_stumble");
        
        // Анамнезы Аркадий Скептик
        anamnesisToKey.Add("Отказался от лекарств: \"Если я выздоровею, кто будет смеяться над вашими зарплатами?\"", "anamnesis_arkady_medicine_refusal");
        anamnesisToKey.Add("Сказал, что \"настоящая больница — это IKEA без выхода\"", "anamnesis_arkady_ikea");
        anamnesisToKey.Add("Когда ему предложили шутку, ответил: \"Ага, прямо смешно... как зарплата\".", "anamnesis_arkady_salary_joke");
        
        // Анамнезы Зинаида-Космос
        anamnesisToKey.Add("Говорит, что луна - это лампочку, которую забыли выключить.", "anamnesis_zinaida_moon_bulb");
        anamnesisToKey.Add("Пыталась отправить телеграмму в прошлое через микроволновку.", "anamnesis_zinaida_microwave_telegram");
        anamnesisToKey.Add("Утверждает, что её кот управляет погодой с помощью моргания.", "anamnesis_zinaida_cat_weather");
        
        // Анамнезы Профессор Буквоед
        anamnesisToKey.Add("Называет аптечку \"аптека для чайников\"", "anamnesis_professor_first_aid_kit");
        anamnesisToKey.Add("Пытался написать стихотворение, где каждое слово начинается на \"х\".", "anamnesis_professor_x_poem");
        anamnesisToKey.Add("Смеётся, когда говоришь \"гиппопотам\" вместо \"бегемот\".", "anamnesis_professor_hippopotamus");
        
        // Анамнезы Вера Реалист
        anamnesisToKey.Add("Сказала: \"Смех - лучшее лекарство? Тогда я хочу рецепт на \"хохотин\"\"", "anamnesis_vera_laughter_recipe");
        anamnesisToKey.Add("Когда ей показали воздушный шарик, спросила: \"Он тоже лечит?\"", "anamnesis_vera_balloon");
        anamnesisToKey.Add("Отказывается верить, что врачи - не актёры.", "anamnesis_vera_doctors_actors");
        
        // Анамнезы Борис-Кувырок
        anamnesisToKey.Add("Смотрит видео с падающими людьми и хихикает.", "anamnesis_boris_falling_videos");
        anamnesisToKey.Add("Пытался сделать соль на кровати, но застрял в занавеске.", "anamnesis_boris_salt_bed");
        anamnesisToKey.Add("Говорит, что \"самое смешное - когда кто-то спотыкается о воздух\".", "anamnesis_boris_air_stumble");
        
        // Анамнезы Главврач Грустин (босс)
        anamnesisToKey.Add("Не смеялся ни разу за 37 лет.", "anamnesis_grustin_37_years");
        anamnesisToKey.Add("Его улыбка - это когда он на 0.1 мм приподнимает бровь.", "anamnesis_grustin_eyebrow");
        anamnesisToKey.Add("Говорит: \"Смех — это ошибка нервной системы\".", "anamnesis_grustin_nervous_system");
        anamnesisToKey.Add("Но... в его кабинете стоит плюшевый кот в очках.", "anamnesis_grustin_plush_cat");
        
        Debug.Log("All key mappings initialized!");
        EditorUtility.DisplayDialog("Initialization Complete", 
            $"Initialized {nameToKey.Count} names, {diagnosisToKey.Count} diagnoses, and {anamnesisToKey.Count} anamnesis keys!", "OK");
    }
    
    void UpdateAllProfiles()
    {
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        int updatedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                bool needsUpdate = false;
                string fileName = Path.GetFileNameWithoutExtension(path);
                
                // Добавляем ключ для имени с отладкой
                if (nameToKey.ContainsKey(profile.patientName))
                {
                    profile.patientNameKey = nameToKey[profile.patientName];
                    needsUpdate = true;
                    Debug.Log($"[{fileName}] Found name key: {profile.patientNameKey} for '{profile.patientName}'");
                }
                else
                {
                    Debug.LogWarning($"[{fileName}] No key found for name: '{profile.patientName}'");
                }
                
                // Добавляем ключ для диагноза с отладкой
                if (diagnosisToKey.ContainsKey(profile.diagnosis))
                {
                    profile.diagnosisKey = diagnosisToKey[profile.diagnosis];
                    needsUpdate = true;
                    Debug.Log($"[{fileName}] Found diagnosis key: {profile.diagnosisKey} for '{profile.diagnosis}'");
                }
                else
                {
                    Debug.LogWarning($"[{fileName}] No key found for diagnosis: '{profile.diagnosis}'");
                }
                
                // Добавляем ключи для анамнезов с отладкой
                var anamnesisKeys = new List<string>();
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    string anamnesis = profile.anamnesisLines[i];
                    if (!string.IsNullOrEmpty(anamnesis) && anamnesisToKey.ContainsKey(anamnesis))
                    {
                        anamnesisKeys.Add(anamnesisToKey[anamnesis]);
                        Debug.Log($"[{fileName}] Found anamnesis key {i}: {anamnesisToKey[anamnesis]}");
                    }
                    else
                    {
                        anamnesisKeys.Add(""); // Пустой ключ для неизвестных анамнезов
                        Debug.LogWarning($"[{fileName}] No key found for anamnesis {i}: '{anamnesis}'");
                    }
                }
                
                if (anamnesisKeys.Count > 0)
                {
                    profile.anamnesisKeys = anamnesisKeys.ToArray();
                    needsUpdate = true;
                }
                
                if (needsUpdate)
                {
                    EditorUtility.SetDirty(profile);
                    updatedCount++;
                    Debug.Log($"Updated profile: {fileName}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated {updatedCount} patient profiles!");
        EditorUtility.DisplayDialog("Migration Complete", 
            $"Successfully updated {updatedCount} patient profiles with localization keys!\n\nCheck Unity Console for details.", "OK");
    }
}
