using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Joeys_menu
{
    public class Mods : MonoBehaviour
    {
        public static GameObject rightplat;
       public static GameObject leftplat;
        public static bool isrightplat = false;
        public static bool isleftplat = false;

        public static object Menu { get; private set; }

        public static void SpeedBoost()
        {
            Debug.Log("speed boost has been enabled");
            GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = 12f;
            GorillaLocomotion.GTPlayer.Instance.jumpMultiplier = 12f;
        }

        public static void fly()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position += (GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * 15);
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }

        public static void GhostMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled |= true;
            }
        }
        public static void NoClip()
        {
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f) ;
            bool disableColliders = ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f;
            MeshCollider[] colliders = Resources.FindObjectsOfTypeAll<MeshCollider>();

            foreach (MeshCollider collider in colliders)
            {
                collider.enabled = !disableColliders;
            }

            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position += (GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * 15);
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;

            }
        }

        public static void CarMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position += GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * 15f;
            }

            if (ControllerInputPoller.instance.leftControllerPrimaryButton)
            {
                GorillaLocomotion.GTPlayer.Instance.transform.position -= GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * 5f;
            }

         
        
        }
         public static void Platforms()
        {
            if (ControllerInputPoller.instance.rightGrab && !isrightplat)
            {
                isrightplat = true;
                rightplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rightplat.transform.position = new Vector3(GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position.x, GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position.y + -0.15f, GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position.z);
                rightplat.transform.rotation = Quaternion.identity;
                rightplat.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
                
                Renderer renderer = rightplat.GetComponent<Renderer>();
                renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                renderer.material.color = new Color(180 / 255f, 153 / 255f, 233 / 255f);



            }
            else if (!ControllerInputPoller.instance.rightGrab && isrightplat)
            {
                Destroy(rightplat);
                isrightplat = false;
            }

            if (ControllerInputPoller.instance.leftGrab && !isleftplat)
            {
                isleftplat = true;
                leftplat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leftplat.transform.position = new Vector3(GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position.x, GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position.y + -0.15f, GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position.z);
                leftplat.transform.rotation = Quaternion.identity;
                leftplat.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);

                Renderer renderer = leftplat.GetComponent<Renderer>();
                renderer.material.shader = Shader.Find("GorillaTag/UberShader");
                renderer.material.color = new Color(180 / 255f, 153 / 255f, 233 / 255f);



            }

            else if (!ControllerInputPoller.instance.leftGrab && isleftplat)
            {
                Destroy(leftplat);
                isleftplat = false;
            }

        }

        public static void GrabRig()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                GorillaTagger.Instance.offlineVRRig.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }




        public static void Tracers()
        {
            foreach (VRRig vrrigs in GorillaParent.instance.vrrigs)
            {
                if (!vrrigs.isOfflineVRRig && !vrrigs.isMyPlayer)
                {
                    GameObject line = new GameObject("line");
                    LineRenderer lr = line.AddComponent<LineRenderer>();

                    // Create a new material with shader like BoxESP uses
                    Material mat = new Material(Shader.Find("GUI/Text Shader"));
                    Color color = Color.blue;
                    mat.color = new Color(color.r, color.g, color.b, 0.6f); // Slight transparency like in BoxESP

                    lr.material = mat; // Assign material to LineRenderer

                    lr.startWidth = 0.01f;
                    lr.endWidth = 0.01f;
                    lr.positionCount = 2;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, GTPlayer.Instance.rightControllerTransform.position);
                    lr.SetPosition(1, vrrigs.transform.position);

                    // Destroy the line object and material after a frame to avoid leaks
                    GameObject.Destroy(line, Time.deltaTime);
                    GameObject.Destroy(mat, Time.deltaTime);
                }
            }
        }

        public static void BoxESP()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                {
                    Transform bodyTransform = rig.transform;

                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.name = "BoxESP";

                    // Destroy collider to avoid physics issues
                    GameObject.Destroy(box.GetComponent<BoxCollider>());

                    // Set material so it’s visible through walls
                    Material mat = new Material(Shader.Find("GUI/Text Shader"));
                    mat.color = new Color(0f, 1f, 0f, 0.3f); // Light green, half transparent
                    box.GetComponent<Renderer>().material = mat;

                    // Parent to player body
                    box.transform.SetParent(bodyTransform);

                    // Size the box (adjust to match Gorilla body scale)
                    float height = 0.9f;
                    float width = 0.35f;
                    float depth = 0.2f;
                    box.transform.localScale = new Vector3(width, height, depth);

                    // Position the box so it starts at head and extends downward
                    box.transform.localPosition = new Vector3(0f, -height / 2f, 0f);

                    // Optional slight rotation align
                    box.transform.localRotation = Quaternion.identity;

                    // Destroy after a frame
                    GameObject.Destroy(box, Time.deltaTime);
                }
            }
        }












    }
}
