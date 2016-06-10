using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

public class JSONExporter : MonoBehaviour
{
    private static ILogger logger = Debug.logger;
    private static readonly string className = "JSONExporter";

    public static string SAVE_TO_DIR = @"E:\EDUCATION\Unity3d\PROJECTS\SpriteToJSON\";

    [MenuItem("JSONGenerate/DragonBonesType")]
    [MenuItem("Assets/JSONGenerate/DragonBonesType")]
    static void JSONGenerate_DragonBonesType()
    {
        //logger.Log(className, "Hello");
        //Debug.logger.Log(className, "World");

        Object[] textures = GetSelectedTextures();

        Selection.objects = new Object[0];
        foreach (Texture2D texture in textures)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            string textureTag = ti.spritePackingTag;

            Debug.logger.Log(className, "texture name + type = " + texture.name + " type = " + ti.textureType);
            Debug.logger.Log(className, "texture name = " + textureTag);
            Debug.logger.Log(className, "texture path = " + path);

            /* 
             JsonObject testJSON = new JsonObject(textureTag, path);
             SubTexture subTexture1 = new SubTexture(1, 2, 3, "text_1", 4);
             SubTexture subTexture2 = new SubTexture(11, 22, 33, "text_2", 44);
             SubTexture subTexture3 = new SubTexture(111, 222, 333, "text_3", 444);
             testJSON.addSubTexture(subTexture1);
             testJSON.addSubTexture(subTexture2);
             testJSON.addSubTexture(subTexture3);
             string jsonTest = JsonUtility.ToJson(testJSON);
             Debug.logger.Log(className, "jsonTest = " + jsonTest);
             */

            JsonObject jsonObject = new JsonObject(textureTag, path.Replace("Assets/", ""));
            List<SpriteMetaData> newData = new List<SpriteMetaData>();
            foreach (SpriteMetaData sripteMetaData in ti.spritesheet)
            {
                SpriteMetaData d = sripteMetaData;
                d.alignment = 9;
                d.pivot = ti.spritesheet[0].pivot;
                d.pivot.x = 0;
                newData.Add(d);
                //logger.Log(className, "sprite name:"+d.name+"; "+d.re);
                var rect = d.rect;
                jsonObject.addSubTexture(
                    new SubTexture((int) rect.width, (int) rect.y, (int) rect.height, d.name, (int) rect.x)
                    );
            }
            string jsonFromObject = JsonUtility.ToJson(jsonObject);
            logger.Log(className, "json = " + jsonFromObject);

            int pos = path.IndexOf(".");
            string saveDir = SAVE_TO_DIR + path.Replace("/", "\\").Remove(pos).Insert(pos, ".json");
            logger.Log(className, "saveDir = " + saveDir);

            if (File.Exists(saveDir))
            {
                File.Delete(saveDir);
            }
            File.WriteAllText(saveDir, jsonFromObject);

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
    }

    [System.Serializable]
    class JsonObject
    {
        //List<Person> people = new List<Person>{
        //new Person{ID = 1, FirstName = "Scott", LastName = "Gurthie"},
        //new Person{ID = 2, FirstName = "Bill", LastName = "Gates"}
        //};

        public List<SubTexture> SubTexture = new List<JSONExporter.SubTexture>();
        public string name;
        public string imagePath;

        public JsonObject(string name, string imagePath)
        {
            this.name = name;
            this.imagePath = imagePath;
        }

        public void addSubTexture(params SubTexture[] subTextures)
        {
            foreach (var subTexture in subTextures)
            {
                SubTexture.Add(subTexture);
            }
        }
    }

    [System.Serializable]
    class SubTexture
    {
        /*"SubTexture" : [{
			"width" : 120,
			"y" : 460,
			"height" : 36,
			"name" : "parts/beardL",
			"x" : 661
		}, ...
        */
        public int y;
        public int x;
        public int width;
        public int height;
        public string name;

        public SubTexture(int width, int y, int height, string name, int x)
        {
            this.width = width;
            this.height = height;
            this.x = x;
            this.y = y;
            this.name = name;
        }
    }
}