using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FireworkManager: MonoBehaviour
{
    private static readonly Dictionary<string, string> nameToFileNameDict = new Dictionary<string, string> {
        { "RocketBlueRain1", "Fireworks - Rocket - Blue Rain 1.prefab" },
        { "RocketBlueRain2", "Fireworks - Rocket - Blue Rain 2.prefab" },
        { "RocketBlueRain3", "Fireworks - Rocket - Blue Rain 3.prefab" },
        { "RocketBlueRain4", "Fireworks - Rocket - Blue Rain 4.prefab" },
        { "RocketBlueRain", "Fireworks - Rocket - Blue Rain.prefab" },
        { "RocketBurningTwinkle1", "Fireworks - Rocket - Burning Twinkle 1.prefab" },
        { "RocketBurningTwinkle2", "Fireworks - Rocket - Burning Twinkle 2.prefab" },
        { "RocketBurningTwinkle3", "Fireworks - Rocket - Burning Twinkle 3.prefab" },
        { "RocketBurningTwinkle4", "Fireworks - Rocket - Burning Twinkle 4.prefab" },
        { "RocketBurningTwinkle5", "Fireworks - Rocket - Burning Twinkle 5.prefab" },
        { "RocketBurningTwinkle6", "Fireworks - Rocket - Burning Twinkle 6.prefab" },
        { "RocketBurningTwinkle7", "Fireworks - Rocket - Burning Twinkle 7.prefab" },
        { "RocketBurningTwinkle", "Fireworks - Rocket - Burning Twinkle.prefab" },
        { "RocketPinkSnapCrackle1", "Fireworks - Rocket - Pink SnapCrackle 1.prefab" },
        { "RocketPinkSnapCrackle", "Fireworks - Rocket - Pink SnapCrackle.prefab" },
        { "RocketRainbowSparkle1", "Fireworks - Rocket - Rainbow Sparkle 1.prefab" },
        { "RocketRainbowSparkle2", "Fireworks - Rocket - Rainbow Sparkle 2.prefab" },
        { "RocketRainbowSparkle3", "Fireworks - Rocket - Rainbow Sparkle 3.prefab" },
        { "RocketRainbowSparkle", "Fireworks - Rocket - Rainbow Sparkle.prefab" },
        { "RocketSimple1", "Fireworks - Rocket - Simple 1.prefab" },
        { "RocketSimple2", "Fireworks - Rocket - Simple 2.prefab" },
        { "RocketSimple", "Fireworks - Rocket - Simple.prefab" }
    };


    private struct FireworkClass
    {
        private static readonly string fileRoot = "Assets/Fireworks/Prefabs/";

        public string name;
        public string fileName;
        public Object constructor;

        public FireworkClass (string aName, string aFileName)
        {
            this.name = aName;
            this.fileName = aFileName;                
            string path = fileRoot + fileName;
            this.constructor = AssetDatabase.LoadAssetAtPath (path, typeof(GameObject));               
        }
    };

    private Dictionary<string, FireworkClass> fireworkClassDict =
        new Dictionary<string, FireworkClass> ();

    void initFireworkClassDict ()
    {
        foreach (KeyValuePair<string, string> entry in nameToFileNameDict) {
            string name = entry.Key;
            string fileName = entry.Value;
            FireworkClass firework = new FireworkClass (name, fileName);
            fireworkClassDict.Add (name, firework);
        }
    }

    private class FireworkInstance
    {
        private Object constructor;

        public GameObject obj = null;
        public string name = "";
        public float startTime = 0.0f;
        public float duration = 0.0f;
        public Vector3 startPostion = Vector3.zero;

        public FireworkInstance (Object aConstructor)
        {
            this.constructor = aConstructor;
        }

        public void show ()
        {
            if (obj == null) {               
                obj = Object.Instantiate (constructor, startPostion, Quaternion.identity) as GameObject;
            }
        }

        public void destroy ()
        {
            if (obj != null) {
                Object.Destroy (obj);
            }
        }

        ~FireworkInstance ()
        {
            destroy ();
        }
    };

    private List<FireworkInstance> instances = new List<FireworkInstance> ();

    void addFireworkInstance (string name, float startTime, float duration, Vector3 startPosition)
    {
        FireworkClass firework;
        if (!fireworkClassDict.TryGetValue (name, out firework)) {
            return;
        }

        Object constructor = firework.constructor;
        FireworkInstance instance = new FireworkInstance (constructor);
        instance.name = name;
        instance.startTime = startTime;
        instance.duration = duration;
        instance.startPostion = startPosition;

        instances.Add (instance);
    }

    void addFireworkInstance (JSONObject firework)
    {
        if (firework.type != JSONObject.Type.OBJECT)
            return;

        string name;
        float startTime, duration;
        Vector3 position;

        if (firework.GetField ("name").type == JSONObject.Type.STRING) {
            name = firework.GetField ("name").str;
        } else {
            return;
        }

        if (firework.GetField ("start_time").type == JSONObject.Type.NUMBER) {
            startTime = firework.GetField ("start_time").n;
        } else {
            return;
        }

        if (firework.GetField ("duration").type == JSONObject.Type.NUMBER) {
            duration = firework.GetField ("duration").n;
        } else {
            return;
        }

        if (firework.GetField ("position").type == JSONObject.Type.OBJECT) {
            JSONObject p = firework.GetField ("position");
            if (p.type == JSONObject.Type.OBJECT) {
                if (p.GetField ("x").type != JSONObject.Type.NUMBER)
                    return;
                if (p.GetField ("y").type != JSONObject.Type.NUMBER)
                    return;
                if (p.GetField ("z").type != JSONObject.Type.NUMBER)
                    return;

                position.x = p.GetField ("x").n;
                position.y = p.GetField ("y").n;
                position.z = p.GetField ("z").n;
            } else {
                return;
            }
        } else {
            return;
        }

        addFireworkInstance (name, startTime, duration, position); 
    }

    string readFileAsString (string fileName)
    {
        TextAsset data= Resources.Load(fileName) as TextAsset;
        return data.text;
    }

    void loadFireworksFromJSONString (string jsonStr)
    {        
        JSONObject json = new JSONObject (jsonStr);

        if (json.type == JSONObject.Type.OBJECT) {
            JSONObject fireworks = json.GetField ("fireworks");
            if (fireworks.type == JSONObject.Type.ARRAY) {
                foreach (JSONObject firework in fireworks.list) {
                    addFireworkInstance (firework);
                }
            }             
        }
    }

    void Start ()
    {
        initFireworkClassDict ();

        string jsonStr = readFileAsString("data/demo0");       
        loadFireworksFromJSONString (jsonStr);
    }

    void Update ()
    {
        float now = Time.realtimeSinceStartup;

        foreach (FireworkInstance instance in instances) {
            if (now >= instance.startTime && now < instance.startTime + instance.duration) {
                instance.show ();
            } else {
                instance.destroy ();
            }
        }
    }
        
}
