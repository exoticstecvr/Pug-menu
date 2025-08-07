using BepInEx;
using BepInEx.Configuration;
using GorillaExtensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Joeys_menu.Class1;
using static ThrowableBug;
namespace Joeys_menu
{
    [BepInPlugin("Joey.menu", "Joeys awsome menu", "0.0.1")]
    public class Class1 : BaseUnityPlugin
    {

        public float pageSwitchCoolDown;

        public int currentCategoryIndex = -1;
        public int currentPageIndex = 0;

        List<List<List<string>>> allCategories = new List<List<List<string>>>();
        List<string> categoryNames = new List<string> { "Movement", "Extra" };
        bool menuimput;
        bool IsMenuCreated;

        GameObject menuObj;


        List<GameObject> btnsObjs = new List<GameObject>();

        public ConfigEntry<bool> speedBoostEnabled;
        public ConfigEntry<bool> flyEnabled;
        public ConfigEntry<bool> ghostMonkeyEnabled;
        public ConfigEntry<bool> getUserIDEnabled;
        public ConfigEntry<bool> disconnectEnabled;
        public ConfigEntry<bool> noClipEnabled;
        public ConfigEntry<bool> carMonkeyEnabled;
        public ConfigEntry<bool> platformsEnabled;
        public ConfigEntry<bool> grabRigEnabled;
        public ConfigEntry<bool> tracersEnabled;
        public ConfigEntry<bool> boxESPEnabled;
        public ConfigEntry<bool> gunEnabled;
        public ConfigEntry<bool> joyStickFlyEnabled;
        public ConfigEntry<bool> ironMonkeyEnabled;
        public static Class1 instance;
        void Awake()


        {

            List<List<string>> movementPages = new List<List<string>>
            {
                new List<string> {"Fly", "Speed Boost", "Ghost Monkey", "NoClip(LG)", "Car Monkey", "Platforms"},
                 new List<string> {"Grab Rig", "RidePlayerGun", "JoyStickFly", "IronMonkey" }
            };

            List<List<string>> extraPages = new List<List<string>>
            {
                new List<string> { "Tracers", "Box ESP"},   

            };
            


            allCategories.Add(movementPages);
            allCategories.Add(extraPages);
            instance = this;
            Harmony harmony = new Harmony("Joey.menu");
            harmony.PatchAll();

            speedBoostEnabled = Config.Bind("Settings", "Speed Boost Enabled", false, "Toggle Speed Boost");
            ghostMonkeyEnabled = Config.Bind("Settings", "Ghost Monkey Enabled", false, "Toggle Ghost Monkey");
            flyEnabled = Config.Bind("Settings", "Fly Enabled", false, "Toggle Fly");
            disconnectEnabled = Config.Bind("Settings", "Disconnect Enabled", false, "Toggle Disconnect");
            noClipEnabled = Config.Bind("Settings", "NoClip(LG) Enabled", false, "Toggle noClip");
            carMonkeyEnabled = Config.Bind("Settings", "Car Monkey Enabled", false, "Toggle Car Monkey");
            platformsEnabled = Config.Bind("Settings", "Platforms Enabled", false, "Toggle Platforms");
            grabRigEnabled = Config.Bind("Settings", "Grab Rig Enabled", false, "Toggle Grab Rig");
            tracersEnabled = Config.Bind("Settings", "Tracers Enabled", false, "Toggle Tracers");
            boxESPEnabled = Config.Bind("Settings", "BoxEsp Enabled", false, "Toggle Boxesp");
            gunEnabled = Config.Bind("Settings", "RidePlaye Gun Enabled", false, "Toggle gun");
            joyStickFlyEnabled = Config.Bind("Settings", "JoyStickFly Enabled", false, "Toggle JoyStickFly");
            ironMonkeyEnabled = Config.Bind("Settings", "IronMonkey Enabled", false, "Toggle IronMonkey");



        }
        public static bool ZG;
        void FixedUpdate()
        {

            if (ZG)
            {
                GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
                GorillaTagger.Instance.rigidbody.velocity = Vector3.zero;
            }
            ZG = false;
        }
   
        void Start()
        {
            IsMenuCreated = false;
        }

        void Update()
        {



            menuimput = ControllerInputPoller.instance.leftControllerSecondaryButton;

            if (pageSwitchCoolDown > 0f)
                pageSwitchCoolDown -= Time.deltaTime;

            if (menuimput && !IsMenuCreated)
            {
                CreateMenu();
                Debug.Log("pressed");
            }
            else if (!menuimput && IsMenuCreated)
            {
                DestroyMenu();
            }

            if (speedBoostEnabled.Value) Mods.SpeedBoost();
            if (ghostMonkeyEnabled.Value) Mods.GhostMonkey();
            if (flyEnabled.Value) Mods.fly();
            if (noClipEnabled.Value) Mods.NoClip();
            if (carMonkeyEnabled.Value) Mods.CarMonkey();
            if (platformsEnabled.Value) Mods.Platforms();
            if (grabRigEnabled.Value) Mods.GrabRig();
            if (tracersEnabled.Value) Mods.Tracers();
            if (boxESPEnabled.Value) Mods.BoxESP();
            if (gunEnabled.Value) Mods.RidePlayerGun();
            if (joyStickFlyEnabled.Value) Mods.Joystickfly();
            if (ironMonkeyEnabled.Value) Mods.IronMonkey();

        }

        void CreateMenu()
        {

            AddDisconnect(0f, "Disconnect", 0.3f, 0f);
            GorillaLocomotion.GTPlayer player = GorillaLocomotion.GTPlayer.Instance;

            IsMenuCreated = true;


            menuObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            menuObj.transform.parent = player.leftControllerTransform;
            menuObj.transform.localPosition = Vector3.zero;
            menuObj.transform.rotation = player.leftControllerTransform.rotation;


            menuObj.transform.localScale = new Vector3(0.03f, 0.35f, 0.45f);

            Renderer rend = menuObj.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("GorillaTag/UberShader");
            rend.material.color = Color.blue;
            var textObject = new GameObject("MenuLabel");
            textObject.transform.SetParent(menuObj.transform);
            textObject.transform.localPosition = new Vector3(0.55f, 0f, 0.43f);
            textObject.transform.localRotation = Quaternion.Euler(0f, -90f, -90f);

            var text = textObject.AddComponent<TextMeshPro>();
            text.text = "Pug Menu";
            text.fontSize = 10;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;
            text.rectTransform.sizeDelta = new Vector2(10f, 10f);
            text.transform.localScale = new Vector3(0.1f, 0.05f, 0.1f);

            //left is x aka depth
            // middle is y up and down
            //right is z aka left and right


            GameObject.Destroy(menuObj.GetComponent<Collider>());
            GameObject.Destroy(menuObj.GetComponent<Rigidbody>());

            if (currentCategoryIndex == -1)
            {
                float yoffset = 0.1f;

                foreach (string categoryName in categoryNames)
                {
                    Addbutton(yoffset, categoryName, 0.2f, 0f);
                    yoffset -= 0.05f;
                }
                return;
            }
            List<string> currentButton = allCategories[currentCategoryIndex][currentPageIndex];
            float offset = -0.1f;

            foreach (string btnName in currentButton)
            {
                Addbutton(offset, btnName, 0.3f, 0f);
                offset -= -0.05f;
            }

            Addbutton(-0.15f, "<<", 0.1f, 0.06f);
            Addbutton(-0.15f, ">>", 0.1f, -0.06f);
            Addbutton(-0.1f, "Back", 0.1f, 0.135f);
            

        }
        
        void DestroyMenu()
        {
            Config.Save();
            GameObject.Destroy(menuObj);
            IsMenuCreated = false;
            DestroyButton();

        }

        void Addbutton(float zoffset, string btnName, float soffset, float yoffset)
        {
            var player = GorillaLocomotion.GTPlayer.Instance;
            GameObject btnObj;
            btnObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var follow = btnObj.AddComponent<Followmenu>();
            follow.position = new Vector3(0.035f, yoffset, zoffset);
            follow.rotation = Quaternion.identity;
            follow.target = player.leftControllerTransform;


            btnObj.layer = 18;

            //Left is x
            //Middle is y
            //Right is z

            btnObj.transform.localScale = new Vector3(0.03f, soffset, 0.04f);



            Renderer renderer = btnObj.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("GorillaTag/UberShader");
            renderer.material.color = new Color(0.7f, 0.85f, 0.95f);
            var trigger = btnObj.AddComponent<ButtanActivation>();

            trigger.btnIdentifier = btnName;



            btnsObjs.Add(btnObj);

            var textObject = new GameObject("ButtonLabel");
            textObject.transform.SetParent(btnObj.transform);
            textObject.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            textObject.transform.localRotation = Quaternion.Euler(0f, -90f, -90f);

            var text = textObject.AddComponent<TextMeshPro>();
            text.text = btnName;
            text.fontSize = 30;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;
            text.enableAutoSizing = true;
            text.rectTransform.sizeDelta = new Vector2(50f, 40f);
            text.transform.localScale = new Vector3(0.01f, 0.1f, 0.3f);



        }

        void AddDisconnect(float zoffset, string btnName, float soffset, float yoffset)
        {
            var player = GorillaLocomotion.GTPlayer.Instance;
            GameObject btnObj;
            btnObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var follow = btnObj.AddComponent<Followmenu>();
            follow.position = new Vector3(0.035f, yoffset, 0.25f);
            follow.rotation = Quaternion.identity;
            follow.target = player.leftControllerTransform;


            btnObj.layer = 18;

            //Left is x
            //Middle is y
            //Right is z

            btnObj.transform.localScale = new Vector3(0.03f, soffset, 0.04f);



            Renderer renderer = btnObj.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find("GorillaTag/UberShader");
            renderer.material.color = new Color(0.7f, 0.85f, 0.95f);
            var trigger = btnObj.AddComponent<ButtanActivation>();

            trigger.btnIdentifier = btnName;



            btnsObjs.Add(btnObj);

            var textObject = new GameObject("ButtonLabel");
            textObject.transform.SetParent(btnObj.transform);
            textObject.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            textObject.transform.localRotation = Quaternion.Euler(0f, -90f, -90f);

            var text = textObject.AddComponent<TextMeshPro>();
            text.text = btnName;
            text.fontSize = 30;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.black;
            text.enableAutoSizing = true;
            text.rectTransform.sizeDelta = new Vector2(50f, 40f);
            text.transform.localScale = new Vector3(0.01f, 0.1f, 0.3f);



        }

        void DestroyButton()
        {
            foreach (GameObject btnObj in btnsObjs)
            {
                Destroy(btnObj);
            }
            btnsObjs.Clear();
        }

        void NextPage()
        {
            List<List<string>> currentCategory = allCategories[currentCategoryIndex];
            currentPageIndex = (currentCategoryIndex + 1) % currentCategory.Count;
            DestroyMenu();
            CreateMenu();
        }

        void PreviousPage()
        {
            List<List<string>> currentCategory = allCategories[currentCategoryIndex];
            currentPageIndex = (currentPageIndex - 1 + currentCategory.Count) % currentCategory.Count;

            DestroyMenu();
            CreateMenu();
        }


        public class ButtanActivation : GorillaPressableButton
        {
            public string btnIdentifier;
            bool IsToggled;
            bool IsToggleAble;

            void Start()
            {
                switch (btnIdentifier)
                {
                    case "Speed Boost":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.speedBoostEnabled.Value;
                        break;

                    case "Fly":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.flyEnabled.Value;
                        break;

                    case "Ghost Monkey":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.ghostMonkeyEnabled.Value;
                        break;

                    case "NoClip(LG)":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.noClipEnabled.Value;
                        break;

                    case "Car Monkey":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.carMonkeyEnabled.Value;
                        break;

                    case "Platforms":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.platformsEnabled.Value;
                        break;

                    case "Grab Rig":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.grabRigEnabled.Value;
                        break;

                    case "Tracers":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.tracersEnabled.Value;
                        break;

                    case "Box ESP":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.boxESPEnabled.Value;
                        break;

                    case "RidePlayerGun":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.gunEnabled.Value;
                        break;

                    case "JoyStickFly":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.joyStickFlyEnabled.Value;
                        break;

                    case "IronMonkey":
                        IsToggleAble = true;
                        IsToggled = Class1.instance.ironMonkeyEnabled.Value;
                        break;

                    case "Disconnect":
                        IsToggleAble = false;
                        IsToggled = Class1.instance.disconnectEnabled.Value;
                        break;

                }
                if (IsToggleAble)
                {
                    GetComponent<Renderer>().material.color = IsToggled ? Color.green : new Color(0.7f, 0.85f, 0.95f);
                }
            }
            public override void ButtonActivationWithHand(bool isLeftHand)
            {
                base.ButtonActivationWithHand(isLeftHand);

                if (!isLeftHand)
                {

                    if (IsToggleAble)
                    {
                        IsToggled = !IsToggled;
                        GetComponent<Renderer>().material.color = IsToggled ? Color.green : new Color(0.7f, 0.85f, 0.95f);
                    }
                    switch (btnIdentifier)
                    {
                        case "Speed Boost":
                            Class1.instance.speedBoostEnabled.Value = IsToggled;
                            Mods.SpeedBoost();
                            break;

                        case "Fly":
                            Class1.instance.flyEnabled.Value = IsToggled;
                            break;

                        case "Ghost Monkey":
                            Class1.instance.ghostMonkeyEnabled.Value = IsToggled;
                            Mods.GhostMonkey();
                            break;

                        case "NoClip(LG)":
                            IsToggled = Class1.instance.noClipEnabled.Value = IsToggled;
                            break;

                        case "Car Monkey":
                            IsToggled = Class1.instance.carMonkeyEnabled.Value = IsToggled;
                            break;

                        case "Platforms":
                            IsToggled = Class1.instance.platformsEnabled.Value = IsToggled;
                            break;

                        case "Grab Rig":
                            IsToggled = Class1.instance.grabRigEnabled.Value = IsToggled;
                            break;

                        case "Tracers":
                            IsToggled = Class1.instance.tracersEnabled.Value = IsToggled;
                            break;

                        case "Box ESP":
                            IsToggled = Class1.instance.boxESPEnabled.Value = IsToggled;
                            break;

                        case "RidePlayerGun":
                            IsToggled = Class1.instance.gunEnabled.Value = IsToggled;
                            break;

                        case "JoyStickFly":
                            IsToggled = Class1.instance.joyStickFlyEnabled.Value = IsToggled;
                            break;

                        case "IronMonkey":
                            IsToggled = Class1.instance.ironMonkeyEnabled.Value = IsToggled;
                            break;

                        case "Disconnect":
                            Mods.Disconnect();
                            break;


                        case "<<":
                            if (Class1.instance.pageSwitchCoolDown <= 0)
                            {
                                Class1.instance.PreviousPage();
                                Class1.instance.pageSwitchCoolDown = 0.3f;
                            }
                            break;

                        case ">>":
                            if (Class1.instance.pageSwitchCoolDown <= 0)
                            {
                                Class1.instance.NextPage();
                                Class1.instance.pageSwitchCoolDown = 0.3f;
                            }
                            break;

                        case "Movement":
                            Class1.instance.currentCategoryIndex = 0;
                            Class1.instance.currentPageIndex = 0;
                            Class1.instance.DestroyMenu();
                            Class1.instance.CreateMenu();
                            break;

                        case "Extra":
                            Class1.instance.currentCategoryIndex = 1;
                            Class1.instance.currentPageIndex = 0;
                            Class1.instance.DestroyMenu();
                            Class1.instance.CreateMenu();
                            break;

                        case "Back":
                            Class1.instance.currentCategoryIndex = -1;
                            Class1.instance.currentPageIndex = 0;
                            Class1.instance.DestroyMenu();
                            Class1.instance.CreateMenu();
                            break;
                    }
                    if (IsToggleAble)
                    Class1.instance.Config.Save();

                }
            }
        }

        public class Followmenu : MonoBehaviour
        {
            public Transform target;
            public Vector3 position;
            public Quaternion rotation;

            void LateUpdate()
            {
                transform.position = target.TransformPoint(position);
                transform.rotation = target.rotation * rotation;
            }
        }





    }
}
