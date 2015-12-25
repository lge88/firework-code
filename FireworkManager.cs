using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

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

    void initFireworkClassDict() {
        foreach (KeyValuePair<string, string> entry in nameToFileNameDict) {
            string name = entry.Key;
            string fileName = entry.Value;
            FireworkClass firework = new FireworkClass (name, fileName);
            fireworkClassDict.Add (name, firework);
        }
    }        

    private class FireworkInstance {
        private Object constructor;

        public GameObject obj = null;
        public string name = "";
        public float startTime = 0.0f;
        public float duration = 0.0f;
        public Vector3 startPostion = Vector3.zero;

        public FireworkInstance(Object aConstructor) {
            this.constructor = aConstructor;
        }

        public void show() {
            if (obj == null) {               
                obj = Object.Instantiate (constructor, startPostion, Quaternion.identity) as GameObject;
            }
        }

        public void destroy() {
            if (obj != null) {
                Object.Destroy(obj);
            }
        }

        ~FireworkInstance() {
            destroy();
        }
    };

    private List<FireworkInstance> instances = new List<FireworkInstance>();

    void addFireworkInstance (string name, float startTime, float duration, Vector3 startPosition)
    {
        FireworkClass firework;
        if (!fireworkClassDict.TryGetValue (name, out firework)) {
            return;
        }

        Object constructor = firework.constructor;
        FireworkInstance instance = new FireworkInstance(constructor);
        instance.startTime = startTime;
        instance.duration = duration;
        instance.startPostion = startPosition;

        instances.Add (instance);
    }

    void loadFireworkInstances() {
        addFireworkInstance("RocketBlueRain1", 2.0f, 5.0f, Vector3.zero);
        addFireworkInstance("RocketRainbowSparkle2", 4.0f, 5.0f, Vector3.zero);
    }
        
    void Start ()
    {
        initFireworkClassDict();
        loadFireworkInstances();
    }        
        
    void Update ()
    {
        float now = Time.realtimeSinceStartup;

        foreach (FireworkInstance instance in instances) {
            if (now >= instance.startTime && now < instance.startTime + instance.duration) {
                instance.show();
            } else {
                instance.destroy();
            }
        }
    }
        
}
