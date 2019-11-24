﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KerbalKonstructs.Core;
using UnityEngine;
using UnityEngine.UI;

namespace KerbalKonstructs.UI2
{
    internal class HangarKSCGUI
    {

        internal static PopupDialog dialog;
        internal static MultiOptionDialog optionDialog;
        internal static List<DialogGUIBase> content;

        internal static string windowName = "KSCHangar";
        internal static string windowMessage = null;
        internal static string windowTitle = "All stored Vessels";

        internal static Rect windowRect;

        internal static float windowWidth = 350f;
        internal static float windowHeight = 200f;

        internal static bool showTitle = false;
        internal static bool showKKTitle = true;
        internal static bool isModal = false;


        internal static bool setToParent = false;

        internal static bool checkForParent = false;
        internal static Func<bool> parentWindow = KSCManager.IsOpen;


        internal static void CreateContent()
        {
            content.Add(new DialogGUILabel("SpaceCenter,   VesselName", KKStyle.whiteLabel));
            content.Add(VaiantList);
        }


        internal static void KKTitle()
        {
            if (!showKKTitle)
            {
                return;
            }
            content.Add(new DialogGUIHorizontalLayout(
                new DialogGUILabel("-KK-", KKStyle.windowTitle),
                new DialogGUIFlexibleSpace(),

                new DialogGUILabel(windowTitle, KKStyle.windowTitle),
                new DialogGUIFlexibleSpace(),
                new DialogGUIButton("X", delegate { Close(); }, 21f, 21.0f, true, KKStyle.DeadButtonRed)

                ));
        }



        internal static DialogGUIScrollList VaiantList
        {
            get
            {
                List<DialogGUIBase> list = new List<DialogGUIBase>();
                list.Add(new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true));
                list.Add(new DialogGUIFlexibleSpace());
                //list.Add(new DialogGUIButton("Default", delegate { SetVariant(null);}, 140.0f, 30.0f, true));

                foreach (var staticInstance in StaticDatabase.allStaticInstances)
                {
                    if (!staticInstance.hasFacilities || staticInstance.facilityType != Modules.KKFacilityType.Hangar)
                    {
                        continue;
                    }

                    Modules.Hangar hangar = staticInstance.GetFacility(Modules.KKFacilityType.Hangar) as Modules.Hangar;
                    foreach (Modules.Hangar.StoredVessel storedVessel in hangar.storedVessels)
                    {
                        Modules.Hangar.StoredVessel vessel = storedVessel;
                        list.Add(new DialogGUIHorizontalLayout(
                            new DialogGUILabel(staticInstance.groupCenterName, KKStyle.whiteLabel) ,
                            new DialogGUIFlexibleSpace(),
                            new DialogGUILabel(vessel.vesselName),
                            new DialogGUIFlexibleSpace(),
                            new DialogGUIButton("Launch", delegate { LaunchVessel(vessel, hangar); }, 60.0f, 23.0f, true))
                            );
                    }
                }
                list.Add(new DialogGUIFlexibleSpace());
                list.Add(new DialogGUIButton("Close", delegate { Close(); }, false));
                var layout = new DialogGUIVerticalLayout(10, 100, 4, new RectOffset(6, 24, 10, 10), TextAnchor.MiddleCenter, list.ToArray());
                var scroll = new DialogGUIScrollList(new Vector2(350, 200), new Vector2(330, 23f * list.Count-2), false, true, layout);
                return scroll;

            }
        }



        internal static void LaunchVessel(Modules.Hangar.StoredVessel vessel, Modules.Hangar hangar)
        {
            Vessel newVessel = Modules.Hangar.RollOutVessel(vessel, hangar);

            if (newVessel == null)
            {
                Log.UserError("Count not receive Vessel from Storage. Now its gone forever!");

            }

            // remove the control lock. which was created by the KSC Manager window
            InputLockManager.RemoveControlLock("KK_KSC");

            GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
            FlightDriver.StartAndFocusVessel("persistent", FlightGlobals.Vessels.IndexOf(newVessel));
        }



        internal static void CreateMultiOptionDialog()
        {
            windowRect = new Rect(WindowManager.GetPosition(windowName), new Vector2(windowWidth, windowHeight));
            optionDialog = new MultiOptionDialog(windowName, windowMessage, windowTitle, null, windowRect, content.ToArray());
            optionDialog.OnFixedUpdate += PlaceToParent;
        }


        internal static void CreatePopUp()
        {
            dialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                   new Vector2(0.5f, 0.5f), optionDialog,
                   false,
                   null, isModal);
            if (!showTitle)
            {
                dialog.gameObject.GetChild("Title").SetActive(false);
            }
            if (checkForParent)
            {
                dialog.dialogToDisplay.OnUpdate += CheckForParent;
            }
        }

        internal static void PlaceToParent()
        {
            Log.Normal(" " + dialog.dialogToDisplay.position.ToString());
        }

        internal static void CheckForParent()
        {
            if (checkForParent)
            {
                if (parentWindow != null && !parentWindow.Invoke())
                {
                    Close();
                }
            }
        }



        internal static void Open()
        {
            KKStyle.Init();
            //windowRect = new Rect(CreateBesidesMainwindow(), new Vector2(windowWidth, windowHeight));
            content = new List<DialogGUIBase>();
            KKTitle();
            CreateContent();
            CreateMultiOptionDialog();
            CreatePopUp();

            

        }


        internal static void Close()
        {
            if (dialog != null)
            {
                WindowManager.SavePosition(dialog);
                dialog.Dismiss();
            }
            dialog = null;
            optionDialog = null;
        }


        internal static bool isOpen
        {
            get
            {
                return (dialog != null);
            }
        }

        internal static bool IsOpen()
        {
            return (dialog != null);
        }

        internal static bool IsClosed()
        {
            return (dialog == null);
        }

        internal static void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }


        internal static void SetVariant(string variantName)
        {
            Log.Normal("Base Selected: " + variantName);
        }


    }
}