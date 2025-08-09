using Fusion;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using Valve.VR;

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
            GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = 12f;
            GorillaLocomotion.GTPlayer.Instance.jumpMultiplier = 12f;
        }


        public static void fly()
        {
            bool triggerPressed = ControllerInputPoller.instance.rightControllerSecondaryButton;
            Rigidbody rb = GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>();

            if (triggerPressed)
            {
                isFlying = true;

                Vector3 flyDirection = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward;
                float flySpeed = 15f;

                rb.velocity = flyDirection * flySpeed;
            }
            else if (isFlying)
            {
                // Stop flying smoothly when trigger released
                rb.velocity = Vector3.zero;
                isFlying = false;
            }
        }


        private static bool ghostMode = false;
        private static float lastToggleTime = 0f;
        private static float toggleCooldown = 1f; // 1 second cooldown

        public static void GhostMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton && Time.time - lastToggleTime > toggleCooldown)
            {
                ghostMode = !ghostMode;  // Toggle ghost mode
                GorillaTagger.Instance.offlineVRRig.enabled = !ghostMode;  // Disable rig when ghostMode is true

                lastToggleTime = Time.time;  // Update last toggle time
            }
        }
        public static void NoClip()
        {
            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f)
            {
                MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
                foreach (MeshCollider meshCollider in array)
                {
                    meshCollider.enabled = false;
                }
            }
            else
            {
                MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
                foreach (MeshCollider meshCollider in array)
                {
                    meshCollider.enabled = true;
                }
            }
        }
    


        private static Vector3 currentVelocity = Vector3.zero;
        private const float acceleration = 20f;
        private const float maxSpeed = 15f;

        public static void CarMonkey()
        {
            Rigidbody rb = GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>();
            if (rb == null) return;

            Vector3 forward = GorillaLocomotion.GTPlayer.Instance.headCollider.transform.forward;
            Vector3 inputDirection = Vector3.zero;

            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                inputDirection += forward;
            }

            if (ControllerInputPoller.instance.leftControllerPrimaryButton)
            {
                inputDirection -= forward * 0.1f; 
            }

            inputDirection = inputDirection.normalized;

            
            Vector3 targetVelocity = inputDirection * maxSpeed;

            
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

            
            Vector3 nextPosition = rb.position + currentVelocity * Time.deltaTime;

            
            nextPosition.y = rb.position.y;

            
            rb.MovePosition(nextPosition);
        }

        public static bool scaleWithPlayer = false; // Default: don't scale with size
        public static int flySpeedCycle = 1;
        public static float _flySpeed = 10f;

        public static float flySpeed
        {
            get => _flySpeed * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
            set => _flySpeed = value;
        }

        public static void IronMonkey()
        {
            Rigidbody rb = GorillaTagger.Instance.rigidbody;

            if (ControllerInputPoller.instance.leftGrab)
            {
                Vector3 leftForce = flySpeed * -GorillaTagger.Instance.leftHandTransform.right;
                rb.AddForce(leftForce * Time.deltaTime, ForceMode.VelocityChange);

                float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 50f * rb.velocity.magnitude;
                GorillaTagger.Instance.StartVibration(true, hapticStrength, GorillaTagger.Instance.tapHapticDuration);
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                Vector3 rightForce = flySpeed * GorillaTagger.Instance.rightHandTransform.right;
                rb.AddForce(rightForce * Time.deltaTime, ForceMode.VelocityChange);

                float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 50f * rb.velocity.magnitude;
                GorillaTagger.Instance.StartVibration(false, hapticStrength, GorillaTagger.Instance.tapHapticDuration);
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


                    Material mat = new Material(Shader.Find("GUI/Text Shader"));
                    Color color = Color.blue;
                    mat.color = new Color(color.r, color.g, color.b, 0.6f); 

                    lr.material = mat; 

                    lr.startWidth = 0.01f;
                    lr.endWidth = 0.01f;
                    lr.positionCount = 2;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, GTPlayer.Instance.rightControllerTransform.position);
                    lr.SetPosition(1, vrrigs.transform.position);

                    
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


        public static GameObject lineObj;
        public static LineRenderer line;
        public static GameObject targetSphere;
        public static VRRig lockedTarget;
        public static bool gripHeld = false;
        public static bool isFlying = false;
        public static void RidePlayerGun()
        {
            bool gripDown = ControllerInputPoller.instance.rightControllerGripFloat > 0.5f;
            bool triggerPressed = ControllerInputPoller.instance.rightControllerIndexFloat > 0.3;
            Transform hand = GorillaTagger.Instance.rightHandTransform;

            if (gripDown)
            {
                if (!gripHeld)
                {
                    gripHeld = true;

                    lineObj = new GameObject("GunLine");
                    line = lineObj.AddComponent<LineRenderer>();
                    Material mat = new Material(Shader.Find("GUI/Text Shader"));
                    mat.color = new Color(0f, 0.5f, 1f, 0.8f);
                    line.material = mat;
                    line.startWidth = 0.01f;
                    line.endWidth = 0.01f;
                    line.positionCount = 2;
                    line.useWorldSpace = true;

                    targetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    targetSphere.name = "TargetSphere";
                    GameObject.Destroy(targetSphere.GetComponent<Collider>());
                    Material sphereMat = new Material(Shader.Find("GUI/Text Shader"));
                    sphereMat.color = new Color(0f, 0.5f, 1f, 0.3f);
                    targetSphere.GetComponent<Renderer>().material = sphereMat;
                    targetSphere.transform.localScale = Vector3.one * 0.3f;

                    lockedTarget = null;
                }

                Vector3 start = hand.position;
                Vector3 direction = hand.forward;
                Vector3 end = start + direction * 10000f;

                RaycastHit[] hits = Physics.RaycastAll(start, direction, 10000f);

                VRRig closestPlayerHit = null;
                float closestDistance = Mathf.Infinity;
                Vector3 closestPoint = end;

                foreach (var hit in hits)
                {
                    Collider col = hit.collider;
                    MeshCollider meshCol = col as MeshCollider;
                    bool isEnabledMesh = (meshCol != null && meshCol.enabled);

                    VRRig rig = col.GetComponentInParent<VRRig>();
                    if ((isEnabledMesh || rig != null) && hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestPoint = hit.point;

                        if (rig != null && !rig.isOfflineVRRig && !rig.isMyPlayer)
                        {
                            closestPlayerHit = rig;
                        }
                    }
                }

                if (lockedTarget != null)
                {
                    Vector3 headPos = lockedTarget.head?.rigTarget?.position ?? lockedTarget.transform.position;
                    end = headPos;
                    targetSphere.transform.position = headPos;
                }
                else
                {
                    end = closestPoint;
                    targetSphere.transform.position = closestPoint;

                    if (gripDown && triggerPressed && closestPlayerHit != null)
                    {
                        lockedTarget = closestPlayerHit;
                    }
                }

                if (triggerPressed && lockedTarget != null)
                {
                    foreach (VRRig rig in GorillaParent.instance.vrrigs)
                    {
                        if (!rig.isOfflineVRRig && !rig.isMyPlayer)
                        {
                            isFlying = true;

                            Vector3 headPos = lockedTarget.head?.rigTarget?.position ?? lockedTarget.transform.position;
                            Vector3 playerPos = GorillaLocomotion.GTPlayer.Instance.transform.position;

                            float flySpeed = 10f;
                            Vector3 newPos = Vector3.MoveTowards(playerPos, headPos, flySpeed * Time.deltaTime);
                            GorillaLocomotion.GTPlayer.Instance.transform.position = newPos;

                            Rigidbody rb = GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>();
                            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.2f);
                        }
                    }
                }
                else
                {
                    isFlying = false;
                }

                line.SetPosition(0, start);
                line.SetPosition(1, end);
            }
            else if (gripHeld)
            {
                gripHeld = false;
                isFlying = false;

                if (lineObj != null) GameObject.Destroy(lineObj);
                if (targetSphere != null) GameObject.Destroy(targetSphere);

                lineObj = null;
                line = null;
                targetSphere = null;
                lockedTarget = null;
            }
        }




        

        public static void Joystickfly()
        {
            var leftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
            var rightJoystick = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);

            
            bool joystickActive = leftJoystick.magnitude >= 0.1f || Mathf.Abs(rightJoystick.y) >= 0.1f;

            if (!joystickActive)
            {
                Class1.ZG = true; 
                return;
            }

           
            Class1.ZG = false;

            
            GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);

            
            Vector3 inputDirection = new Vector3(leftJoystick.x, rightJoystick.y, leftJoystick.y);

            Vector3 playerForward = GTPlayer.Instance.bodyCollider.transform.forward;
            playerForward.y = 0;
            Vector3 playerRight = GTPlayer.Instance.bodyCollider.transform.right;
            playerRight.y = 0;

            Vector3 velocity = inputDirection.x * playerRight + inputDirection.y * Vector3.up + inputDirection.z * playerForward;
            velocity *= GTPlayer.Instance.scale * 10f;

            GorillaTagger.Instance.rigidbody.velocity = Vector3.Lerp(GorillaTagger.Instance.rigidbody.velocity, velocity, 0.12875f);
        }


        public static void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }
        public static void FixRigHandRotation()
        {
            VRRig.LocalRig.leftHand.rigTarget.transform.rotation *= Quaternion.Euler(VRRig.LocalRig.leftHand.trackingRotationOffset);
            VRRig.LocalRig.rightHand.rigTarget.transform.rotation *= Quaternion.Euler(VRRig.LocalRig.rightHand.trackingRotationOffset);
        }
        public static void DoADance()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig.LocalRig.enabled = false;

                Vector3 bodyOffset = (GorillaTagger.Instance.bodyCollider.transform.right * (Mathf.Cos((float)Time.frameCount / 20f) * 0.3f)) + (new Vector3(0f, Mathf.Abs(Mathf.Sin((float)Time.frameCount / 20f) * 0.2f), 0f));
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.bodyCollider.transform.position + new Vector3(0f, 0.15f, 0f) + bodyOffset;
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.bodyCollider.transform.rotation;

                VRRig.LocalRig.leftHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.forward * 0.2f + VRRig.LocalRig.transform.right * -0.4f + VRRig.LocalRig.transform.up * (0.3f + (Mathf.Sin((float)Time.frameCount / 20f) * 0.2f));
                VRRig.LocalRig.rightHand.rigTarget.transform.position = VRRig.LocalRig.transform.position + VRRig.LocalRig.transform.forward * 0.2f + VRRig.LocalRig.transform.right * 0.4f + VRRig.LocalRig.transform.up * (0.3f + (Mathf.Sin((float)Time.frameCount / 20f) * -0.2f));

                VRRig.LocalRig.leftHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;
                VRRig.LocalRig.rightHand.rigTarget.transform.rotation = VRRig.LocalRig.transform.rotation;

                FixRigHandRotation();
            }
            else
                VRRig.LocalRig.enabled = true;
        }

        public static Vector3 lastPosition = Vector3.zero;

        public static void TeleportPlayer(Vector3 position) // Teleports the player to a given position
        {
            GTPlayer.Instance.TeleportTo(position, GTPlayer.Instance.transform.rotation);
            Mods.lastPosition = position;
        }

        // Stores recorded positions and related data
        private static List<object[]> playerPositions = new List<object[]>();

        public static void Rewind()
        {
            // If holding right grip, go backwards in stored positions
            if (ControllerInputPoller.instance.rightControllerGripFloat > 0.1f)
            {
                if (playerPositions.Count > 0)
                {
                    object[] targetPos = playerPositions[^1];

                    // Teleport to the saved root player position
                    TeleportPlayer((Vector3)targetPos[0]);

                    // Restore both hand positions and rotations
                    GorillaTagger.Instance.leftHandTransform.position = (Vector3)targetPos[1];
                    GorillaTagger.Instance.leftHandTransform.rotation = (Quaternion)targetPos[2];

                    GorillaTagger.Instance.rightHandTransform.position = (Vector3)targetPos[3];
                    GorillaTagger.Instance.rightHandTransform.rotation = (Quaternion)targetPos[4];

                    // Restore velocity (reversed for rewind effect)
                    GorillaTagger.Instance.rigidbody.velocity = (Vector3)targetPos[5] * -1f;

                    // Remove the used frame
                    playerPositions.RemoveAt(playerPositions.Count - 1);
                }
            }
            else
            {
                // Save the current state — using player root transform position instead of body collider
                playerPositions.Add(new object[] {
            GTPlayer.Instance.transform.position, // FIXED: matches TeleportTo

            GorillaTagger.Instance.leftHandTransform.position,
            GorillaTagger.Instance.leftHandTransform.rotation,

            GorillaTagger.Instance.rightHandTransform.position,
            GorillaTagger.Instance.rightHandTransform.rotation,

            GorillaTagger.Instance.rigidbody.velocity
        });

                // Limit history to avoid memory bloat
                if (playerPositions.Count > 8640)
                    playerPositions.RemoveAt(0);
            }
        }
    }
}
