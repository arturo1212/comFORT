using Oculus.Voice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognized;
}

[System.Serializable]
public struct HandData
{
    public OVRSkeleton skeleton;
    public List<Gesture> gestures;
    public float threshold; // 0.1f
    public GameObject shotPrefab;
    public GameObject ammoPrefab;
    public Transform thumb_tip;
    public Transform index_tip;
    public bool agarrando;
    //private List<OVRBone> fingerBones;
}

[System.Serializable]
public struct UIObjects
{
    public GameObject thumb_mic_left;
    public GameObject thumb_mic_right;
    public GameObject shout_mic_center;
    public GameObject help_ingame;
    public GameObject reality_cube;
    public GameObject reality_portals;
}

public enum GamePhase
{
    Reality = 0,
    Building = 1,
    Playing = 2
}

public class GestureDetector : MonoBehaviour
{
    CushionSpawner cushionSpawner;
    public GamePhase gamePhase = GamePhase.Reality;
    public HandData hand_left;
    public HandData hand_right;
    public UIObjects indicators;
    public MyTerrain terrain;
    public bool isTraining = false;
    public Transform head_pose;

    public AppVoiceExperience _voiceExperience;
    public List<Vector3> floor_points;
    public bool was_fist;
    int aux = 0;
    public float timeRemaining = 2;
    // Start is called before the first frame update
    bool startedTalking = false;
    void Start()
    {
        cushionSpawner = GetComponent<CushionSpawner>();
        floor_points = new List<Vector3>();
        indicators.shout_mic_center.SetActive(false);
    }


    public void ChooseCorner()
    {
        RaycastHit hit;
        if (Physics.Raycast(head_pose.position, head_pose.TransformDirection(Vector3.forward), out hit))
        {
            Debug.DrawRay(head_pose.position, head_pose.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Gesture currentGesture_L = Recognize(hand_left);
            Gesture currentGesture_R = Recognize(hand_right);
            if ((currentGesture_L.name == "ThumbsUp" || currentGesture_R.name == "ThumbsUp") && !was_fist && timeRemaining <= 0)
            {
                terrain.corners[aux] = hit.point;
                Instantiate(hand_left.ammoPrefab, hit.point, Quaternion.identity);
                was_fist = true;
                timeRemaining = 2;
                aux++;
                aux %= 4;
                if (aux % 4 == 0)
                {
                    //StartGAME()
                    Destroy(indicators.reality_cube);
                    terrain.GenerateTerrain(); //   StartGame linea 1
                    gamePhase = GamePhase.Building; // Startgame linea 2

                }
                // Instanciar efecto piedrero en esa posicion (Que no se muera)
            }
            if (!(currentGesture_L.name == "ThumbsUp" || currentGesture_R.name == "ThumbsUp"))
            {
                was_fist = false;
            }
            timeRemaining -= Time.deltaTime;
        }
    }



    void FixedUpdate()
    {
        switch (gamePhase)
        {
            case GamePhase.Reality:
                ChooseCorner();
                break;
            case GamePhase.Building:
                GetPillows();
                break;
        }
    }


    void Initialize()
    {
        // Deactivate all ingame elements
        _voiceExperience.Deactivate();
        //indicators.thumb_mic_left.SetActive(false);
        //indicators.thumb_mic_right.SetActive(false);
        indicators.shout_mic_center.SetActive(false);
    }

    #region Building Functions

    void InBuilding()
    {
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);
        // Grabbing
        IsGrabbing(ref hand_left, currentGesture_L);
        IsGrabbing(ref hand_right, currentGesture_R);
        StartGame(currentGesture_L, currentGesture_R);
        // TODO (Optional): Add Voice Commands (Reset)
    }
    void IsGrabbing(ref HandData hand, Gesture gesture)
    {
        if (gesture.name.Contains("Fist"))
        {
            hand.agarrando = true;
        }
        else
        {
            hand.agarrando = false;
        }
    }

    public void GetPillows()
    {
        //TODO: Use finger to raycast.
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);
        if (currentGesture_L.name == "Rock" || currentGesture_R.name == "Rock")
        {
            RaycastHit hit;
            if (Physics.Raycast(head_pose.position, head_pose.TransformDirection(Vector3.forward), out hit))
            {
                cushionSpawner.DropPillow(hit.point);
            }
        }
    }

    public void StartGame(Gesture gesture_l, Gesture gesture_r)
    {
        if (gesture_l.name == "ThumbsUp" && gesture_r.name == "ThumbsUp")
        {
            gamePhase = GamePhase.Playing;
            indicators.reality_portals.SetActive(true);
            FindObjectOfType<SacameElMunieco>().jugando = true;
            //TODO: Start generating enemies and so on
        }
    }
    #endregion

    #region Playing Functions
    void InGame()
    {
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);

        bool voice_cond1 = currentGesture_L.name == "Pistol" || currentGesture_R.name == "Pistol";                       // Condition to listen for shooting
        bool voice_cond2 = currentGesture_L.name.Contains("halftalk") && currentGesture_R.name.Contains("halftalk");     // Condition to listen for shouting
        bool voice_cond3 = !_voiceExperience.Active && !_voiceExperience.IsRequestActive && !_voiceExperience.MicActive; // Condition to avoid reactivating voice

        if ((voice_cond1 || voice_cond2) && voice_cond3)
        {
            _voiceExperience.Deactivate();
            _voiceExperience.ActivateImmediately();
            indicators.shout_mic_center.SetActive(true);
        }
        else if (!(voice_cond1 || voice_cond2))
        {
            indicators.shout_mic_center.SetActive(false);
        }
        if (voice_cond2)
        {
            startedTalking = true;
        }
    }

    void ShowHelp(GamePhase phase = GamePhase.Playing)
    {
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);
        if (currentGesture_L.name == "phone" || currentGesture_R.name == "phone")
        {
            switch (phase){
                case GamePhase.Playing:
                indicators.help_ingame.SetActive(true);
                break;
            }
        }
        else
        {
            switch (phase)
            {
                case GamePhase.Playing:
                    indicators.help_ingame.SetActive(false);
                    break;
            }
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (isTraining)
        {
            TrainGesture();
        }
        switch (gamePhase)
        {
            case GamePhase.Reality:
                ChooseCorner();
                Initialize();
                break;
            case GamePhase.Building:
                InBuilding();
                break;
            case GamePhase.Playing:
                InGame();
                ShowHelp(GamePhase.Playing);
                break;
        }
    }

    #region Gesture CRUD
    void SaveGesture(OVRSkeleton skeleton, List<Gesture> gestures)
    {
        var fingerBones = new List<OVRBone>(skeleton.Bones);    // Get Hand Bones
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            // Finger position relative to root
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerData = data;
        gestures.Add(g);
    }

    Gesture Recognize(HandData handdata)
    {
        var fingerBones = new List<OVRBone>(handdata.skeleton.Bones);
        Gesture currentGesture = new Gesture(); // Create empty gesture
        currentGesture.name = "";
        float currentMin = Mathf.Infinity;
        foreach (var gesture in handdata.gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = handdata.skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if (distance > handdata.threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }
            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }

    void TrainGesture()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveGesture(hand_left.skeleton, hand_left.gestures);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveGesture(hand_right.skeleton, hand_right.gestures);
        }
    }
    #endregion

    public void FirePistol(string[] values)
    {
        // TODO: Use a nice prefab
        Vector3 bullet_offset = new Vector3(0.2f, 0.2f, 0.2f);
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);
        if(currentGesture_L.name == "Pistol")
        {
            // Get finger direction
            var amazingEffect = Instantiate(hand_left.shotPrefab, hand_left.index_tip.position, hand_left.shotPrefab.transform.rotation);
            amazingEffect.transform.SetParent(hand_left.index_tip);
            var projectile = Instantiate(hand_left.ammoPrefab, hand_left.index_tip.position + bullet_offset, hand_left.index_tip.rotation);
            projectile.GetComponent<Rigidbody>().AddForce(-projectile.transform.right * 30);
            // TODO: Activate shooting animation and sound 
        }
        if (currentGesture_R.name == "Pistol")
        {
            var amazingEffect = Instantiate(hand_right.shotPrefab, hand_right.index_tip.position, hand_right.shotPrefab.transform.rotation);
            amazingEffect.transform.SetParent(hand_right.index_tip);

            var projectile = Instantiate(hand_right.ammoPrefab, hand_right.index_tip.position + bullet_offset, hand_right.index_tip.rotation);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.right * 30);
            // TODO: Activate shooting animation and sound 
        }
    }

    public void Shout(string[] values)
    {
        Gesture currentGesture_L = Recognize(hand_left);
        Gesture currentGesture_R = Recognize(hand_right);
        Vector3 bullet_offset = new Vector3(0.2f, 0.0f, 0.0f);
        Debug.Log("Auxilio OUT");

        if (startedTalking)
        {
            Debug.Log("Auxilio1");
            Debug.Log("Auxilio " + values[0] + " " + values[1]);
            // Command Our army
            if(values[0] == "pushback")
            {
                var amazingEffect = Instantiate(hand_left.shotPrefab, head_pose.position + bullet_offset, Quaternion.identity);
                amazingEffect.transform.SetParent(head_pose);
                // Get isVisible enemies and scare For each (Enemy.Scare())
                EnemyBehaviour[] enemies = FindObjectsOfType<EnemyBehaviour>();
                int count = 0;
                foreach(EnemyBehaviour behav in enemies)
                {
                    if (behav.gameObject.activeInHierarchy && behav.gameObject.GetComponent<Renderer>().isVisible)
                        behav.Scare();
                    count++;
                }
                Debug.Log("PUSHING BACK " + count);


            }
            if (values[0] == "movement")
            {
                AllyBehaviour[] allies = FindObjectsOfType<AllyBehaviour>();
                foreach (AllyBehaviour behav in allies)
                {
                    Cardinals result = (Cardinals)Enum.Parse(typeof (Cardinals), values[1], true);
                    behav.SetCardinal(result);
                }
                // Get if isVisible Army (at least one)
            }
            startedTalking = false;
        }
    }


}
