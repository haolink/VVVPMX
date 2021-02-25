using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PMXStructure.PMXClasses;
using System.Globalization;
using System.IO;

namespace VVVPMX
{
    public class BoneData
    {
        public bool Prefixed = true;
        public bool Hidden = false;
        public bool Movable = false;
        public string OrgName = null;
        public string JpName = null;
        public string EnName = null;
        public string ParentJp = null;
        public string ChildJp = null;
        public string Group = null;
        public PMXVector3 Direction = null;
    }

    public class MorphData
    {
        public string OrgName = null;
        public string JpName = null;
    }

    public class IKInfo
    {
        public string[] BoneNames;
        public string IKLegJp;
        public string IKLegEn;
        public string IKFootJp;
        public string IKFootEn;
    }

    public class ExtraGroup
    {
        public string GroupName;
        public string[] GroupKeys;
    }

    class Program
    {
        static void Main(string[] args)
        {
            string inname = @"F:\rip\nvs\convert\C830\C830.pmx";
            string outname = @"F:\rip\nvs\mmd\ileheart\ileheart.pmx";

            string modelName = "Ileheart";
            string modelComment = "Ripped from VVVTune! Model is made by Idea Factory and Compile Heart, ported to MMD by haoLink.\r\n\r\nFeel free to use, please respect the original makers though!";

            bool prefixDefault = false;

            BoneData[] bones = new BoneData[]
            {
                //Body
                new BoneData() { Prefixed = false, OrgName="jointroot_000", EnName="Master", JpName="全ての親", Movable = true },
                new BoneData() { OrgName="ce_center", EnName="lower_root", JpName="下半身R", ParentJp="センター", Hidden = true },
                new BoneData() { OrgName="ce_hip", EnName="lower", JpName="下半身", Direction = new PMXVector3(0, -1.0f, 0), Group = "Body" },
                new BoneData() { OrgName="center_neutral_spine_001", EnName="lback", JpName="上半身", ChildJp="上半身２", ParentJp = "センター", Group = "Body" },
                new BoneData() { OrgName="center_neutral_spine_002", EnName="uback", JpName="上半身２", ChildJp="上半身３", Group = "Body" },
                new BoneData() { OrgName="center_neutral_spine_003", EnName="thorax", JpName="上半身３", ChildJp="首", Group = "Body" },
                new BoneData() { OrgName="center_neutral_neck_001", EnName="neck", JpName="首", ChildJp="頭", Group = "Body" },
                new BoneData() { OrgName="center_neutral_head", EnName="head", JpName="頭", Direction = new PMXVector3(0, 2.0f, 0), Group = "Body" },

                //Eyes
                new BoneData() { OrgName="left_neutral_eyeseyesyes_001", EnName="eyes", JpName="両目", Group = "Body" },
                new BoneData() { OrgName="left_neutral_eyerot_001", EnName="eye_l", JpName="左目", ChildJp="左目E", Group = "Body" },
                new BoneData() { OrgName="left_neutral_eye_001", Hidden = true, JpName="左目E" },
                new BoneData() { OrgName="right_neutral_eyerot_001", EnName="eye_r", JpName="右目", ChildJp="右目E", Group = "Body" },
                new BoneData() { OrgName="right_neutral_eye_001", Hidden = true, JpName="右目E" },

                //Left arm
                new BoneData() { OrgName="left_top_shoulder", EnName="shoulder_l", JpName="左肩", ChildJp="左腕", Group = "Arms" },
                new BoneData() { OrgName="left_top_arm", EnName="arm_l", JpName="左腕", ChildJp="左ひじ", Group = "Arms" },
                new BoneData() { OrgName="left_top_armrot", Hidden=true },
                new BoneData() { OrgName="left_top_elbowtrans", Hidden=true },
                new BoneData() { OrgName="left_top_elbow", EnName="elbow_l", JpName="左ひじ", ChildJp="左手首", Group = "Arms" },
                new BoneData() { OrgName="left_top_wristrot", Hidden=true },
                new BoneData() { OrgName="left_top_hand", EnName="hand_l", JpName="左手首", Direction = new PMXVector3(0.6f, 0, 0), Group = "Arms" },

                //Right arm
                new BoneData() { OrgName="right_top_shoulder", EnName="shoulder_r", JpName="右肩", ChildJp="右腕", Group = "Arms" },
                new BoneData() { OrgName="right_top_arm", EnName="arm_r", JpName="右腕", ChildJp="右ひじ", Group = "Arms" },
                new BoneData() { OrgName="right_top_armrot", Hidden=true },
                new BoneData() { OrgName="right_top_elbowtrans", Hidden=true },
                new BoneData() { OrgName="right_top_elbow", EnName="elbow_r", JpName="右ひじ", ChildJp="右手首", Group = "Arms" },
                new BoneData() { OrgName="right_top_wristrot", Hidden=true },
                new BoneData() { OrgName="right_top_hand", EnName="hand_r", JpName="右手首", Direction = new PMXVector3(-0.6f, 0, 0), Group = "Arms" },

                //Left thumb
                new BoneData() { OrgName="left_top_thumb_001", JpName="左親指０", EnName="thumb_L1", ChildJp="左親指１", Group = "Fingers" },
                new BoneData() { OrgName="left_top_thumb_002", JpName="左親指１", EnName="thumb_L2", ChildJp="左親指２", Group = "Fingers" },
                new BoneData() { OrgName="left_top_thumb_003", JpName="左親指２", EnName="thumb_L3", ChildJp="左親指E", Group = "Fingers" },
                new BoneData() { OrgName="left_top_thumb_004", JpName="左親指E", EnName="thumb_LE", Hidden=true },
                //Left index finger
                new BoneData() { OrgName="left_top_index_001", JpName="左人指０", EnName="index_L1", ChildJp="左人指１", Group = "Fingers" },
                new BoneData() { OrgName="left_top_index_002", JpName="左人指１", EnName="index_L2", ChildJp="左人指２", Group = "Fingers" },
                new BoneData() { OrgName="left_top_index_003", JpName="左人指２", EnName="index_L3", ChildJp="左人指３", Group = "Fingers" },
                new BoneData() { OrgName="left_top_index_004", JpName="左人指３", EnName="index_L4", ChildJp="左人指E", Group = "Fingers" },
                new BoneData() { OrgName="left_top_index_005", JpName="左人指E", EnName="index_LE", Hidden=true },
                //Left middle finger
                new BoneData() { OrgName="left_top_middle_001", JpName="左中指０", EnName="middle_L1", ChildJp="左中指１", Group = "Fingers" },
                new BoneData() { OrgName="left_top_middle_002", JpName="左中指１", EnName="middle_L2", ChildJp="左中指２", Group = "Fingers" },
                new BoneData() { OrgName="left_top_middle_003", JpName="左中指２", EnName="middle_L3", ChildJp="左中指３", Group = "Fingers" },
                new BoneData() { OrgName="left_top_middle_004", JpName="左中指３", EnName="middle_L4", ChildJp="左中指E", Group = "Fingers" },
                new BoneData() { OrgName="left_top_middle_005", JpName="左中指E", EnName="middle_LE", Hidden=true },
                //Left ring finger
                new BoneData() { OrgName="left_top_ring_001", JpName="左薬指０", EnName="ring_L1", ChildJp="左薬指１", Group = "Fingers" },
                new BoneData() { OrgName="left_top_ring_002", JpName="左薬指１", EnName="ring_L2", ChildJp="左薬指２", Group = "Fingers" },
                new BoneData() { OrgName="left_top_ring_003", JpName="左薬指２", EnName="ring_L3", ChildJp="左薬指３", Group = "Fingers" },
                new BoneData() { OrgName="left_top_ring_004", JpName="左薬指３", EnName="ring_L4", ChildJp="左薬指E", Group = "Fingers" },
                new BoneData() { OrgName="left_top_ring_005", JpName="左薬指E", EnName="ring_LE", Hidden=true },
                //Left pinkie
                new BoneData() { OrgName="left_top_pinky_001", JpName="左小指０", EnName="pinkie_L1", ChildJp="左小指１", Group = "Fingers" },
                new BoneData() { OrgName="left_top_pinky_002", JpName="左小指１", EnName="pinkie_L2", ChildJp="左小指２", Group = "Fingers" },
                new BoneData() { OrgName="left_top_pinky_003", JpName="左小指２", EnName="pinkie_L3", ChildJp="左小指３", Group = "Fingers" },
                new BoneData() { OrgName="left_top_pinky_004", JpName="左小指３", EnName="pinkie_L4", ChildJp="左小指E", Group = "Fingers" },
                new BoneData() { OrgName="left_top_pinky_005", JpName="左小指E", EnName="pinkie_LE", Hidden=true },

                //Right thumb
                new BoneData() { OrgName="right_top_thumb_001", JpName="右親指０", EnName="thumb_R1", ChildJp="右親指１", Group = "Fingers" },
                new BoneData() { OrgName="right_top_thumb_002", JpName="右親指１", EnName="thumb_R2", ChildJp="右親指２", Group = "Fingers" },
                new BoneData() { OrgName="right_top_thumb_003", JpName="右親指２", EnName="thumb_R3", ChildJp="右親指E", Group = "Fingers" },
                new BoneData() { OrgName="right_top_thumb_004", JpName="右親指E", EnName="thumb_RE", Hidden=true },
                //Right index finger
                new BoneData() { OrgName="right_top_index_001", JpName="右人指０", EnName="index_R1", ChildJp="右人指１", Group = "Fingers" },
                new BoneData() { OrgName="right_top_index_002", JpName="右人指１", EnName="index_R2", ChildJp="右人指２", Group = "Fingers" },
                new BoneData() { OrgName="right_top_index_003", JpName="右人指２", EnName="index_R3", ChildJp="右人指３", Group = "Fingers" },
                new BoneData() { OrgName="right_top_index_004", JpName="右人指３", EnName="index_R4", ChildJp="右人指E", Group = "Fingers" },
                new BoneData() { OrgName="right_top_index_005", JpName="右人指E", EnName="index_RE", Hidden=true },
                //Right middle finger
                new BoneData() { OrgName="right_top_middle_001", JpName="右中指０", EnName="middle_R1", ChildJp="右中指１", Group = "Fingers" },
                new BoneData() { OrgName="right_top_middle_002", JpName="右中指１", EnName="middle_R2", ChildJp="右中指２", Group = "Fingers" },
                new BoneData() { OrgName="right_top_middle_003", JpName="右中指２", EnName="middle_R3", ChildJp="右中指３", Group = "Fingers" },
                new BoneData() { OrgName="right_top_middle_004", JpName="右中指３", EnName="middle_R4", ChildJp="右中指E", Group = "Fingers" },
                new BoneData() { OrgName="right_top_middle_005", JpName="右中指E", EnName="middle_RE", Hidden=true },
                //Right ring finger
                new BoneData() { OrgName="right_top_ring_001", JpName="右薬指０", EnName="ring_R1", ChildJp="右薬指１", Group = "Fingers" },
                new BoneData() { OrgName="right_top_ring_002", JpName="右薬指１", EnName="ring_R2", ChildJp="右薬指２", Group = "Fingers" },
                new BoneData() { OrgName="right_top_ring_003", JpName="右薬指２", EnName="ring_R3", ChildJp="右薬指３", Group = "Fingers" },
                new BoneData() { OrgName="right_top_ring_004", JpName="右薬指３", EnName="ring_R4", ChildJp="右薬指E", Group = "Fingers" },
                new BoneData() { OrgName="right_top_ring_005", JpName="右薬指E", EnName="ring_RE", Hidden=true },
                //Right pinkie
                new BoneData() { OrgName="right_top_pinky_001", JpName="右小指０", EnName="pinkie_R1", ChildJp="右小指１", Group = "Fingers" },
                new BoneData() { OrgName="right_top_pinky_002", JpName="右小指１", EnName="pinkie_R2", ChildJp="右小指２", Group = "Fingers" },
                new BoneData() { OrgName="right_top_pinky_003", JpName="右小指２", EnName="pinkie_R3", ChildJp="右小指３", Group = "Fingers" },
                new BoneData() { OrgName="right_top_pinky_004", JpName="右小指３", EnName="pinkie_R4", ChildJp="右小指E", Group = "Fingers" },
                new BoneData() { OrgName="right_top_pinky_005", JpName="右小指E", EnName="pinkie_RE", Hidden=true },

                //Left leg
                new BoneData() { OrgName="left_neutral_leg_001", JpName="左足", EnName="groin_l", ChildJp="左ひざ", Group = "Legs" },
                new BoneData() { OrgName="left_neutral_leg_002", JpName="左ひざ", EnName="knee_l", ChildJp="左足首", Group = "Legs" },
                new BoneData() { OrgName="left_neutral_foot_001", JpName="左足首", EnName="ankle_l", ChildJp="左つま先", Group = "Legs" },
                new BoneData() { OrgName="left_neutral_foot_002", JpName="左つま先", EnName="toe_l", ChildJp="左つま先E", Group = "Legs" },
                new BoneData() { OrgName="left_neutral_foot_003", JpName="左つま先E", Hidden=true },
                new BoneData() { OrgName="left_neutral_legtrans", Hidden=true },
                new BoneData() { OrgName="left_neutral_legrot", Hidden=true },
                //Right leg
                new BoneData() { OrgName="right_neutral_leg_001", JpName="右足", EnName="groin_r", ChildJp="右ひざ", Group = "Legs" },
                new BoneData() { OrgName="right_neutral_leg_002", JpName="右ひざ", EnName="knee_r", ChildJp="右足首", Group = "Legs" },
                new BoneData() { OrgName="right_neutral_foot_001", JpName="右足首", EnName="ankle_r", ChildJp="右つま先", Group = "Legs" },
                new BoneData() { OrgName="right_neutral_foot_002", JpName="右つま先", EnName="toe_r", ChildJp="右つま先E", Group = "Legs" },
                new BoneData() { OrgName="right_neutral_foot_003", JpName="右つま先E", Hidden=true },
                new BoneData() { OrgName="right_neutral_legtrans", Hidden=true },
                new BoneData() { OrgName="right_neutral_legrot", Hidden=true },


                //Ileheart
                //Body
                new BoneData() { OrgName="character1_hips", EnName="lower", JpName="下半身", ParentJp="センター", Direction = new PMXVector3(0, -1.0f, 0), Group = "Body" },
                new BoneData() { OrgName="character1_spine", EnName="lback", JpName="上半身", ChildJp="上半身２", ParentJp = "センター", Group = "Body" },
                new BoneData() { OrgName="character1_spine1", EnName="uback", JpName="上半身２", ChildJp="上半身３", Group = "Body" },
                new BoneData() { OrgName="character1_spine2", EnName="thorax", JpName="上半身３", ChildJp="首", Group = "Body" },
                new BoneData() { OrgName="character1_neck", EnName="neck", JpName="首", ChildJp="頭", Group = "Body" },
                new BoneData() { OrgName="character1_head", EnName="head", JpName="頭", Direction = new PMXVector3(0, 2.0f, 0), Group = "Body" },

                //Eyes
                new BoneData() { OrgName="left_neutral_eyeseyesyes_001", EnName="eyes", JpName="両目", Group = "Body" },
                new BoneData() { OrgName="left_eye_01", EnName="eye_l", JpName="左目", ChildJp="左目E", Group = "Body" },
                new BoneData() { OrgName="left_eye_02", Hidden = true, JpName="左目E" },
                new BoneData() { OrgName="right_eye_01", EnName="eye_r", JpName="右目", ChildJp="右目E", Group = "Body" },
                new BoneData() { OrgName="right_eye_02", Hidden = true, JpName="右目E" },

                //Left arm
                new BoneData() { OrgName="character1_leftshoulder", EnName="shoulder_l", JpName="左肩", ChildJp="左腕", Group = "Arms" },
                new BoneData() { OrgName="character1_leftarm", EnName="arm_l", JpName="左腕", ChildJp="左ひじ", Group = "Arms" },
                new BoneData() { OrgName="character1_leftforearm", EnName="elbow_l", JpName="左ひじ", ChildJp="左手首", Group = "Arms" },
                new BoneData() { OrgName="character1_lefthand", EnName="hand_l", JpName="左手首", Direction = new PMXVector3(0.6f, 0, 0), Group = "Arms" },

                //Right arm
                new BoneData() { OrgName="character1_rightshoulder", EnName="shoulder_r", JpName="右肩", ChildJp="右腕", Group = "Arms" },
                new BoneData() { OrgName="character1_rightarm", EnName="arm_r", JpName="右腕", ChildJp="右ひじ", Group = "Arms" },
                new BoneData() { OrgName="character1_rightforearm", EnName="elbow_r", JpName="右ひじ", ChildJp="右手首", Group = "Arms" },
                new BoneData() { OrgName="character1_righthand", EnName="hand_r", JpName="右手首", Direction = new PMXVector3(-0.6f, 0, 0), Group = "Arms" },

                //Left thumb
                new BoneData() { OrgName="character1_lefthandthumb1", JpName="左親指０", EnName="thumb_L1", ChildJp="左親指１", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandthumb2", JpName="左親指１", EnName="thumb_L2", ChildJp="左親指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandthumb3", JpName="左親指２", EnName="thumb_L3", ChildJp="左親指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandthumb4", JpName="左親指E", EnName="thumb_LE", Hidden=true },
                //Left index finger
                new BoneData() { OrgName="character1_lefthandindex1", JpName="左人指１", EnName="index_L1", ChildJp="左人指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandindex2", JpName="左人指２", EnName="index_L2", ChildJp="左人指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandindex3", JpName="左人指３", EnName="index_L3", ChildJp="左人指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandindex4", JpName="左人指E", EnName="index_LE", Hidden=true },
                //Left middle finger
                new BoneData() { OrgName="character1_lefthandmiddle1", JpName="左中指１", EnName="middle_L1", ChildJp="左中指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandmiddle2", JpName="左中指２", EnName="middle_L2", ChildJp="左中指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandmiddle3", JpName="左中指３", EnName="middle_L3", ChildJp="左中指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandmiddle4", JpName="左中指E", EnName="middle_LE", Hidden=true },
                //Left ring finger
                new BoneData() { OrgName="character1_lefthandring1", JpName="左薬指１", EnName="ring_L1", ChildJp="左薬指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandring2", JpName="左薬指２", EnName="ring_L2", ChildJp="左薬指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandring3", JpName="左薬指３", EnName="ring_L3", ChildJp="左薬指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandring4", JpName="左薬指E", EnName="ring_LE", Hidden=true },
                //Left pinkie
                new BoneData() { OrgName="character1_lefthandpinky1", JpName="左小指１", EnName="pinkie_L1", ChildJp="左小指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandpinky2", JpName="左小指２", EnName="pinkie_L2", ChildJp="左小指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandpinky3", JpName="左小指３", EnName="pinkie_L3", ChildJp="左小指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_lefthandpinky4", JpName="左小指E", EnName="pinkie_LE", Hidden=true },

                //Right thumb
                new BoneData() { OrgName="character1_righthandthumb1", JpName="右親指０", EnName="thumb_R1", ChildJp="右親指１", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandthumb2", JpName="右親指１", EnName="thumb_R2", ChildJp="右親指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandthumb3", JpName="右親指２", EnName="thumb_R3", ChildJp="右親指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandthumb4", JpName="右親指E", EnName="thumb_RE", Hidden=true },                
                //Right index finger
                new BoneData() { OrgName="character1_righthandindex1", JpName="右人指１", EnName="index_R1", ChildJp="右人指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandindex2", JpName="右人指２", EnName="index_R2", ChildJp="右人指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandindex3", JpName="右人指３", EnName="index_R3", ChildJp="右人指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandindex4", JpName="右人指E", EnName="index_RE", Hidden=true },
                //Right middle finger
                new BoneData() { OrgName="character1_righthandmiddle1", JpName="右中指１", EnName="middle_R1", ChildJp="右中指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandmiddle2", JpName="右中指２", EnName="middle_R2", ChildJp="右中指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandmiddle3", JpName="右中指３", EnName="middle_R3", ChildJp="右中指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandmiddle4", JpName="右中指E", EnName="middle_RE", Hidden=true },
                //Right ring finger
                new BoneData() { OrgName="character1_righthandring1", JpName="右薬指１", EnName="ring_R1", ChildJp="右薬指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandring2", JpName="右薬指２", EnName="ring_R2", ChildJp="右薬指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandring3", JpName="右薬指３", EnName="ring_R3", ChildJp="右薬指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandring4", JpName="右薬指E", EnName="ring_RE", Hidden=true },
                //Right pinkie
                new BoneData() { OrgName="character1_righthandpinky1", JpName="右小指１", EnName="pinkie_R1", ChildJp="右小指２", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandpinky2", JpName="右小指２", EnName="pinkie_R2", ChildJp="右小指３", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandpinky3", JpName="右小指３", EnName="pinkie_R3", ChildJp="右小指E", Group = "Fingers" },
                new BoneData() { OrgName="character1_righthandpinky4", JpName="右小指E", EnName="pinkie_RE", Hidden=true },

                //Left leg
                new BoneData() { OrgName="character1_leftupleg", JpName="左足", EnName="groin_l", ChildJp="左ひざ", Group = "Legs" },
                new BoneData() { OrgName="character1_leftleg", JpName="左ひざ", EnName="knee_l", ChildJp="左足首", Group = "Legs" },
                new BoneData() { OrgName="character1_leftfoot", JpName="左足首", EnName="ankle_l", ChildJp="左つま先", Group = "Legs" },
                new BoneData() { OrgName="character1_lefttoebase", JpName="左つま先", EnName="toe_l", ChildJp="左つま先E", Group = "Legs", Direction = new PMXVector3(0, 0, -0.6f) },                
                //Right leg
                new BoneData() { OrgName="character1_rightupleg", JpName="右足", EnName="groin_r", ChildJp="右ひざ", Group = "Legs" },
                new BoneData() { OrgName="character1_rightleg", JpName="右ひざ", EnName="knee_r", ChildJp="右足首", Group = "Legs" },
                new BoneData() { OrgName="character1_rightfoot", JpName="右足首", EnName="ankle_r", ChildJp="右つま先", Group = "Legs" },
                new BoneData() { OrgName="character1_righttoebase", JpName="右つま先", EnName="toe_r", ChildJp="右つま先E", Group = "Legs", Direction = new PMXVector3(0, 0, -0.6f) },
            };

            MorphData[] mds = new MorphData[]
            {
                new MorphData() { OrgName = "a_big", JpName = "あ" },
                new MorphData() { OrgName = "i_big", JpName = "い" },
                new MorphData() { OrgName = "u_big", JpName = "う" },
                new MorphData() { OrgName = "e_big", JpName = "え" },
                new MorphData() { OrgName = "o_big", JpName = "お" },
                new MorphData() { OrgName = "a_small", JpName = "あ２" },
                new MorphData() { OrgName = "i_small", JpName = "い２" },
                new MorphData() { OrgName = "u_small", JpName = "う２" },
                new MorphData() { OrgName = "e_small", JpName = "え２" },
                new MorphData() { OrgName = "o_small", JpName = "お２" },
                new MorphData() { OrgName = "triangleclose", JpName = "∧" },
                new MorphData() { OrgName = "triangleopen", JpName = "▲" },
                new MorphData() { OrgName = "smile", JpName = "にやり" },
                new MorphData() { OrgName = "a_big2", JpName = "叫び" },
                new MorphData() { OrgName = "cat", JpName = "ω" },
                new MorphData() { OrgName = "square1", JpName = "□" },
                new MorphData() { OrgName = "open_big", JpName = "ワ" },

                new MorphData() { OrgName = "blink_l", JpName = "左まばたき" },
                new MorphData() { OrgName = "blink_r", JpName = "右まばたき" },
                new MorphData() { OrgName = "wink_l", JpName = "左笑い" },
                new MorphData() { OrgName = "wink_r", JpName = "右笑い" },
                new MorphData() { OrgName = "half_l", JpName = "左じと目" },
                new MorphData() { OrgName = "half_r", JpName = "右じと目" },
                new MorphData() { OrgName = "wide_l", JpName = "左びっくり" },
                new MorphData() { OrgName = "wide_r", JpName = "右びっくり" },
                new MorphData() { OrgName = "starcross", JpName = "星目" },
                new MorphData() { OrgName = "hioff", JpName = "HL消" },
                new MorphData() { OrgName = "small", JpName = "瞳小" },

                new MorphData() { OrgName = "up", JpName = "上" },
                new MorphData() { OrgName = "down", JpName = "下" },
                new MorphData() { OrgName = "weak", JpName = "困る" },
                new MorphData() { OrgName = "anger1", JpName = "怒り" },
                new MorphData() { OrgName = "anger2", JpName = "怒り２" },

                new MorphData() { OrgName = "blue", JpName = "真っ青" },
                new MorphData() { OrgName = "cheeksmall", JpName = "照れ" },
                new MorphData() { OrgName = "cheekbig", JpName = "照れ２" },
            };

            IKInfo[] ikis = new IKInfo[]
            {
                new IKInfo() { BoneNames = new string[] { "左足", "左ひざ", "左足首", "左つま先" }, IKLegJp = "左足ＩＫ", IKLegEn = "IK_LLeg", IKFootJp = "左つま先ＩＫ", IKFootEn = "IK_LFoot" },
                new IKInfo() { BoneNames = new string[] { "右足", "右ひざ", "右足首", "右つま先" }, IKLegJp = "右足ＩＫ", IKLegEn = "IK_RLeg", IKFootJp = "右つま先ＩＫ", IKFootEn = "IK_RFoot" },
            };

            ExtraGroup[] exgs = new ExtraGroup[]
            {
                new ExtraGroup() { GroupName = "Hair", GroupKeys = new string[] { "hair" } },
                new ExtraGroup() { GroupName = "Skirt", GroupKeys = new string[] { "skirt" } },
                new ExtraGroup() { GroupName = "Scarf", GroupKeys = new string[] { "scarf" } },
                new ExtraGroup() { GroupName = "Twintails", GroupKeys = new string[] { "twin" } },
                new ExtraGroup() { GroupName = "Body", GroupKeys = new string[] { "bust", "cloth" } },
            };
            
            Dictionary<string, PMXBone> jpBones = new Dictionary<string, PMXBone>();
            string[] prefixes = new string[] { "bdb", "ndb", "fab", "fpb", "dpb", "npb", "loc" };
            List<string> groupNames = new List<string>();

            Dictionary<string, List<PMXBone>> groupBones = new Dictionary<string, List<PMXBone>>();
            

            Dictionary<PMXBone, bool> boneHasVertices = new Dictionary<PMXBone, bool>();

            string[] hiddenPrefixes = new string[] { "ndb", "npb", "loc", "fpb" };

            PMXModel pmxmd = PMXModel.LoadFromPMXFile(inname);

            PMXBone centerBone = new PMXBone(pmxmd);
            centerBone.NameEN = "center";
            centerBone.NameJP = "センター";
            centerBone.Position = new PMXVector3(0.0f, pmxmd.Bones[1].Position.Y * 2.0f/3.0f, 0.0f);
            centerBone.Rotatable = true;
            centerBone.Translatable = true;
            centerBone.Parent = pmxmd.Bones[0];

            pmxmd.Bones.Insert(1, centerBone);

            jpBones.Add("センター", centerBone);

            foreach (PMXBone bn in pmxmd.Bones)
            {
                string orgName = bn.NameJP;
                string innerName = null;

                string prefix = null;
                string group = null;
                int bnIndex = -1;

                string[] groups = orgName.Split(new char[] { '_' });
                if (groups.Length > 1)
                {
                    string testPrefix = groups[0].ToLowerInvariant();
                    if (prefixes.Contains(testPrefix)) 
                    {
                        prefix = testPrefix;

                        innerName = String.Join("_", groups, 1, groups.Length - 1);
                    } 
                    
                    if (prefix != null)
                    {
                        if (hiddenPrefixes.Contains(prefix))
                        {
                            bn.Visible = false;
                        }

                        int testInt = -1;
                        char[] cs = groups[groups.Length - 1].ToCharArray();
                        if (cs.Length == 2 && cs[0] == 'G' && int.TryParse(cs[1].ToString(), out testInt))
                        {
                            group = groups[groups.Length - 1];
                            innerName = String.Join("_", groups, 1, groups.Length - 2);
                        }
                    }

                    if (group != null)
                    {
                        int testInt2 = -1;
                        if (int.TryParse(groups[groups.Length - 2], out testInt2))
                        {
                            bnIndex = testInt2;
                            innerName = String.Join("_", groups, 1, groups.Length - 3);
                        }
                    }                    

                    BoneData bnIdent = null;

                    foreach (BoneData bd in bones)
                    {
                        if (prefixDefault)
                        {
                            if ((!bd.Prefixed) && orgName.ToLowerInvariant() == bd.OrgName.ToLowerInvariant())
                            {
                                bnIdent = bd;
                                break;
                            }
                            if ((bd.Prefixed) && innerName != null && innerName.ToLowerInvariant() == bd.OrgName.ToLowerInvariant())
                            {
                                bnIdent = bd;
                                break;
                            }
                        } 
                        else
                        {
                            if (orgName.ToLowerInvariant() == bd.OrgName.ToLowerInvariant())
                            {
                                bnIdent = bd;
                                break;
                            }
                        }                        
                    }

                    if (bnIdent != null)
                    {
                        if (bnIdent.EnName != null)
                        {
                            bn.NameEN = bnIdent.EnName;
                        }                        
                        if (bnIdent.JpName != null)
                        {
                            bn.NameJP = bnIdent.JpName;

                            if (bnIdent.Group != null && !groupNames.Contains(bnIdent.Group))
                            {
                                groupNames.Add(bnIdent.Group);
                            }
                        }                        

                        bn.Visible = !bnIdent.Hidden;
                        bn.Translatable = bnIdent.Movable;

                        if (bnIdent.ParentJp != null && jpBones.ContainsKey(bnIdent.ParentJp))
                        {
                            bn.Parent = jpBones[bnIdent.ParentJp];
                        }
                        if (bnIdent.Direction != null)
                        {
                            bn.HasChildBone = false;
                            bn.ChildVector = bnIdent.Direction;
                        }

                        if (bnIdent.JpName != null)
                        {
                            jpBones.Add(bn.NameJP, bn);
                        }                        
                    }
                }
            }

            pmxmd.DisplaySlots[0].References.Add(pmxmd.Bones[0]);

            PMXDisplaySlot ikSlot = new PMXDisplaySlot(pmxmd);
            ikSlot.NameJP = "IK";
            ikSlot.NameEN = "IK";
            ikSlot.References.Add(pmxmd.Bones[1]);
            pmxmd.DisplaySlots.Add(ikSlot);

            foreach (IKInfo iki in ikis)
            {
                bool nf = false;
                foreach (string jpName in iki.BoneNames)
                {
                    if (!jpBones.ContainsKey(jpName))
                    {
                        nf = true;
                        break;
                    }
                }
                if (nf)
                {
                    continue;
                }

                PMXBone bnToe = jpBones[iki.BoneNames[3]];
                PMXBone bnFoot = jpBones[iki.BoneNames[2]];

                int indexToe = pmxmd.Bones.IndexOf(bnToe);
                int indexFoot = pmxmd.Bones.IndexOf(bnFoot);                

                PMXBone ikFoot = new PMXBone(pmxmd);
                ikFoot.NameEN = iki.IKFootEn;
                ikFoot.NameJP = iki.IKFootJp;
                ikFoot.Position = new PMXVector3(bnToe.Position.X, bnToe.Position.Y, bnToe.Position.Z);
                ikFoot.Rotatable = true;
                ikFoot.Translatable = true;
                ikFoot.IK = new PMXIK(pmxmd, ikFoot);
                ikFoot.IK.Target = bnToe;
                ikFoot.IK.Loop = 40;
                ikFoot.IK.RadianLimit = (float)(Math.PI / 3);
                PMXIKLink ikl1 = new PMXIKLink(pmxmd, ikFoot.IK);
                ikl1.Bone = bnFoot;
                ikFoot.IK.IKLinks.Add(ikl1);

                PMXBone ikLeg = new PMXBone(pmxmd);
                ikLeg.NameEN = iki.IKLegEn;
                ikLeg.NameJP = iki.IKLegJp;
                ikLeg.Position = new PMXVector3(bnFoot.Position.X, bnFoot.Position.Y, bnFoot.Position.Z);
                ikLeg.Rotatable = true;
                ikLeg.Translatable = true;
                ikLeg.IK = new PMXIK(pmxmd, ikLeg);
                ikLeg.IK.Target = bnFoot;
                ikLeg.IK.Loop = 40;
                ikLeg.IK.RadianLimit = (float)(Math.PI / 3);
                ikl1 = new PMXIKLink(pmxmd, ikLeg.IK);
                ikl1.Bone = jpBones[iki.BoneNames[1]];
                ikl1.HasLimits = true;
                ikl1.Minimum = new PMXVector3((float)((-1) * Math.PI), 0.0f, 0.0f);
                ikl1.Maximum = new PMXVector3((float)(0.5/180.0 * Math.PI), 0.0f, 0.0f);
                PMXIKLink ikl2 = new PMXIKLink(pmxmd, ikLeg.IK);
                ikl2.Bone = jpBones[iki.BoneNames[0]];
                ikLeg.IK.IKLinks.Add(ikl1);
                ikLeg.IK.IKLinks.Add(ikl2);

                pmxmd.Bones.Insert(indexToe + 1, ikFoot);
                pmxmd.Bones.Insert(indexFoot + 1, ikLeg);

                ikLeg.Parent = pmxmd.Bones[0];
                ikLeg.ChildBone = ikFoot;
                ikFoot.Parent = ikLeg;
                ikFoot.HasChildBone = false;
                ikFoot.ChildVector = new PMXVector3(0.0f, 0.0f, -0.5f);

                ikSlot.References.Add(ikLeg);
                ikSlot.References.Add(ikFoot);
            }

            int leftEyeIndex = -1;
            int rightEyeIndex = -1;
            if (jpBones.ContainsKey("左目"))
            {
                leftEyeIndex = pmxmd.Bones.IndexOf(jpBones["左目"]);
            }
            if (jpBones.ContainsKey("右目"))
            {
                rightEyeIndex = pmxmd.Bones.IndexOf(jpBones["右目"]);
            }            
            if (leftEyeIndex >= 0 && rightEyeIndex >= 0 && jpBones.ContainsKey("頭"))
            {                
                int insertIndex = Math.Min(leftEyeIndex, rightEyeIndex);
                PMXBone lEye = pmxmd.Bones[leftEyeIndex];
                PMXBone rEye = pmxmd.Bones[rightEyeIndex];
                PMXBone head = jpBones["頭"];


                PMXBone bothEyes = new PMXBone(pmxmd);
                bothEyes.Position = new PMXVector3(0.0f, lEye.Position.Y + 2.5f, 0.0f);
                bothEyes.NameEN = "eyes";
                bothEyes.NameJP = "両目";
                bothEyes.Parent = head;
                bothEyes.HasChildBone = false;
                bothEyes.ChildVector = new PMXVector3(0.0f, 0.0f, -1.0f);

                pmxmd.Bones.Insert(insertIndex, bothEyes);
                jpBones.Add("両目", bothEyes);

                lEye.HasExternalParent = true;
                lEye.ExternalBone = bothEyes;
                lEye.ExternalBoneEffect = 1.0f;
                lEye.ExternalModificationType = PMXBone.BoneExternalModificationType.Rotation;
                lEye.Layer = 1;

                rEye.HasExternalParent = true;
                rEye.ExternalBone = bothEyes;
                rEye.ExternalBoneEffect = 1.0f;
                rEye.ExternalModificationType = PMXBone.BoneExternalModificationType.Rotation;
                rEye.Layer = 1;

                if (jpBones.ContainsKey("左目E"))
                {
                    jpBones["左目E"].Layer = 1;
                }
                if (jpBones.ContainsKey("右目E"))
                {
                    jpBones["右目E"].Layer = 1;
                }
            }

            foreach (PMXBone bn in pmxmd.Bones)
            {
                boneHasVertices.Add(bn, false);
            }
            foreach (PMXVertex vtx in pmxmd.Vertices)
            {
                if (vtx.Deform is PMXVertexDeformBDEF4)
                {
                    PMXVertexDeformBDEF4 b4 = (PMXVertexDeformBDEF4)(vtx.Deform);
                    if (b4.Bone4 != null && b4.Bone4Weight > 0) boneHasVertices[b4.Bone4] = true;
                    if (b4.Bone3 != null && b4.Bone3Weight > 0) boneHasVertices[b4.Bone3] = true;
                    if (b4.Bone2 != null && b4.Bone2Weight > 0) boneHasVertices[b4.Bone2] = true;
                    if (b4.Bone1 != null && b4.Bone1Weight > 0) boneHasVertices[b4.Bone1] = true;
                }
                else if (vtx.Deform is PMXVertexDeformBDEF2)
                {
                    PMXVertexDeformBDEF2 b2 = (PMXVertexDeformBDEF2)(vtx.Deform);
                    if (b2.Bone2 != null && b2.Bone1Weight < 1) boneHasVertices[b2.Bone2] = true;
                    if (b2.Bone1 != null && b2.Bone1Weight > 0) boneHasVertices[b2.Bone1] = true;
                } 
                else
                {
                    PMXVertexDeformBDEF1 b1 = (PMXVertexDeformBDEF1)(vtx.Deform);
                    boneHasVertices[b1.Bone1] = true;
                }
            }            

            foreach (PMXBone bn in pmxmd.Bones)
            {
                string bnNew = bn.NameJP;
                if (!jpBones.ContainsKey(bnNew))
                {
                    continue;
                }

                BoneData bnIdent = null;

                foreach (BoneData bd in bones)
                {
                    if (bnNew == bd.JpName)
                    {
                        bnIdent = bd;
                        break;
                    }
                }

                if (bnIdent == null || bnIdent.ChildJp == null || !jpBones.ContainsKey(bnIdent.ChildJp))
                {
                    continue;
                }

                bn.HasChildBone = true;
                bn.ChildBone = jpBones[bnIdent.ChildJp];
            }

            

            foreach (PMXBone bn in pmxmd.Bones)
            {
                if (bn.Visible && !BoneHasAssignedVertices(pmxmd, bn, boneHasVertices))
                {
                    bn.Visible = false;
                }
            }

            List<PMXBone> renamedChildBones = new List<PMXBone>();
            for(int i = 0; i < pmxmd.Bones.Count; i++)
            {
                PMXBone bn = pmxmd.Bones[i];

                if (!bn.Visible || renamedChildBones.Contains(bn))
                {
                    continue;
                }

                string orgName = bn.NameJP;
                string innerName = null;

                string prefix = null;
                string group = null;
                int bnIndex = -1;

                string[] groups = orgName.Split(new char[] { '_' });
                if (groups.Length > 1)
                {
                    string testPrefix = groups[0].ToLowerInvariant();
                    if (prefixes.Contains(testPrefix))
                    {
                        prefix = testPrefix;

                        innerName = String.Join("_", groups, 1, groups.Length - 1);
                    }

                    if (prefix != null)
                    {
                        if (hiddenPrefixes.Contains(prefix))
                        {
                            bn.Visible = false;
                        }

                        int testInt = -1;
                        char[] cs = groups[groups.Length - 1].ToCharArray();
                        if (cs.Length == 2 && cs[0] == 'G' && int.TryParse(cs[1].ToString(), out testInt))
                        {
                            group = groups[groups.Length - 1];
                            innerName = String.Join("_", groups, 1, groups.Length - 2);
                        }
                    }

                    int internalIndex = -1;
                    if (group != null)
                    {
                        int testInt2 = -1, testInt3 = -1;
                        if (int.TryParse(groups[groups.Length - 2], out testInt2))
                        {
                            bnIndex = testInt2;
                            innerName = String.Join("_", groups, 1, groups.Length - 3);

                            if (int.TryParse(groups[groups.Length - 3], out testInt3))
                            {
                                innerName = String.Join("_", groups, 1, groups.Length - 4);
                                internalIndex = testInt3;
                            }
                        }
                    }

                    if (!prefixDefault)
                    {
                        int testInt2 = -1;
                        if (int.TryParse(groups[groups.Length - 1], out testInt2)) {
                            int ni = -1;
                            for (int hi = groups.Length - 2; hi >= 0; hi--)
                            {
                                if (groups[hi] == "root")
                                {
                                    continue;
                                }
                                ni = hi;
                                break;
                            }
                            if (ni != -1 && ni > 0)
                            {
                                internalIndex = testInt2;
                                innerName = String.Join("_", groups, 0, ni + 1);
                            }
                        }
                    }

                    if (internalIndex > -1)
                    {                                                
                        List<PMXBone> childBones = FindNameSuitableChildren(bn, innerName, internalIndex, pmxmd);
                        if (childBones != null && childBones.Count > 0)
                        {
                            string groupName = GenerateNewName(innerName);

                            string displaySlot = "Others";
                            foreach (ExtraGroup eg in exgs)
                            {
                                bool found = false;
                                foreach (string key in eg.GroupKeys)
                                {
                                    if (groupName.ToLowerInvariant().Contains(key))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                {
                                    displaySlot = eg.GroupName;
                                    break;
                                }
                            }

                            List<PMXBone> gBones = new List<PMXBone>();

                            if (!groupBones.ContainsKey(displaySlot))
                            {
                                groupBones.Add(displaySlot, gBones);
                            } else
                            {
                                gBones = groupBones[displaySlot];
                            }

                            bn.HasChildBone = true;
                            bn.ChildBone = childBones[0];
                            bn.NameEN = groupName + "1";
                            bn.NameJP = groupName + "1";

                            gBones.Add(bn);

                            if (!groupNames.Contains(displaySlot))
                            {
                                groupNames.Add(displaySlot);
                            }
                            
                            for (int j = 0; j < childBones.Count; j++)
                            {                                
                                if (j < childBones.Count - 1)
                                {
                                    childBones[j].HasChildBone = true;
                                    childBones[j].ChildBone = childBones[j + 1];
                                } 
                                else if(!prefixDefault)
                                {
                                    childBones[j].Visible = false;
                                }
                                childBones[j].NameEN = groupName + (j + 2).ToString();
                                childBones[j].NameJP = groupName + (j + 2).ToString();
                                renamedChildBones.Add(childBones[j]);
                                
                                if (childBones[j].Visible)
                                {
                                    gBones.Add(childBones[j]);
                                }                                
                            }                            
                        }
                    }                    
                }
            }

            foreach (string gn in groupNames)
            {
                PMXDisplaySlot ds = new PMXDisplaySlot(pmxmd);
                ds.NameEN = gn;
                ds.NameJP = gn;
                bool added = false;                

                foreach (BoneData bd in bones)
                {
                    if (bd.Group == null || !jpBones.ContainsKey(bd.JpName) || bd.Group != gn)
                    {
                        continue;
                    }
                    PMXBone bn = jpBones[bd.JpName];
                    
                    if (!bn.Visible)
                    {
                        continue;
                    }

                    if (!added)
                    {
                        pmxmd.DisplaySlots.Add(ds);
                        added = true;
                    }
                    

                    ds.References.Add(bn);
                }

                if (groupBones.ContainsKey(gn))
                {
                    foreach (PMXBone bn in groupBones[gn])
                    {
                        if (!bn.Visible)
                        {
                            continue;
                        }

                        if (!added)
                        {
                            pmxmd.DisplaySlots.Add(ds);
                            added = true;
                        }

                        ds.References.Add(bn);
                    }
                }
            }            

            foreach (PMXMorph mrph in pmxmd.Morphs)
            {
                string name = mrph.NameJP;
                string[] split = name.Split(new char[] { '_' });
                string segment = split[0];

                string jName = String.Join("_", split, 2, split.Length - 2);
                jName = jName.Substring(0, jName.Length - 5);
                mrph.NameJP = jName;
                mrph.NameEN = jName;

                switch (segment)
                {
                    case "eye":
                        mrph.Panel = PMXMorph.PanelType.Eyes;
                        break;
                    case "brow":
                        mrph.Panel = PMXMorph.PanelType.Brows;
                        break;
                    case "mouth":
                        mrph.Panel = PMXMorph.PanelType.Mouth;
                        break;
                    default:
                        mrph.Panel = PMXMorph.PanelType.Other;
                        break;
                }

                foreach (MorphData md in mds)
                {
                    if (jName.ToLowerInvariant() == md.OrgName)
                    {
                        mrph.NameJP = md.JpName;
                        break;
                    }
                }
            }

            CreateMergeMorph(pmxmd, "blink", "まばたき", new string[] { "左まばたき", "右まばたき" });
            CreateMergeMorph(pmxmd, "wink", "笑い", new string[] { "左笑い", "右笑い" });
            CreateMergeMorph(pmxmd, "half", "じと目", new string[] { "左じと目", "右じと目" });
            CreateMergeMorph(pmxmd, "wide", "びっくり", new string[] { "左びっくり", "右びっくり" });            

            foreach (PMXMorph mrph in pmxmd.Morphs)
            {
                pmxmd.DisplaySlots[1].References.Add(mrph);
            }


            if(jpBones.ContainsKey("右腕"))
            {                
                BoneRotator.RotateZ(pmxmd, jpBones["右腕"], 30.0);
                BoneRotator.StraightenHierarchie(pmxmd, jpBones["右腕"]);
            }
            if (jpBones.ContainsKey("左腕"))
            {
                BoneRotator.RotateZ(pmxmd, jpBones["左腕"], -30.0);
                BoneRotator.StraightenHierarchie(pmxmd, jpBones["左腕"]);
            }
            if (jpBones.ContainsKey("右肩"))
            {
                BoneRotator.RotateZ(pmxmd, jpBones["右肩"], 5.0);
                BoneRotator.StraightenHierarchie(pmxmd, jpBones["右肩"]);
            }
            if (jpBones.ContainsKey("左肩"))
            {
                BoneRotator.RotateZ(pmxmd, jpBones["左肩"], -5.0);
                BoneRotator.StraightenHierarchie(pmxmd, jpBones["左肩"]);
            }

            if (modelName != null)
            {
                pmxmd.NameEN = modelName;
                pmxmd.NameJP = modelName;
            }

            if(modelComment != null)
            {
                pmxmd.DescriptionEN = modelComment;
                pmxmd.DescriptionJP = modelComment;
            }

            File_CRC32 crc32 = new File_CRC32();

            string texDirectory = Path.GetDirectoryName(outname) + Path.DirectorySeparatorChar + "tex";
            if (!Directory.Exists(texDirectory))
            {
                Directory.CreateDirectory(texDirectory);
            }
            string[] files = Directory.GetFiles(texDirectory, "*.*");
            List<string> texFiles = new List<string>();

            foreach (string fle in files)
            {
                string flNoExt = Path.GetFileNameWithoutExtension(fle);
                if (flNoExt.Substring(0, 2).ToLowerInvariant() == "tx")
                {
                    texFiles.Add(flNoExt);
                }
            }

            Dictionary<uint, string> outputFileHashes = new Dictionary<uint, string>();
            Dictionary<string, uint> inputFileHashes = new Dictionary<string, uint>();

            foreach (string fullpath in files)
            {
                uint hash = crc32.GetCRC32(fullpath);
                string filename = Path.GetFileName(fullpath);
                outputFileHashes.Add(hash, filename);
            }

            List<string> texFilenames = new List<string>();
            string inputDirectory = Path.GetDirectoryName(inname) + Path.DirectorySeparatorChar;
            foreach (PMXMaterial mat in pmxmd.Materials)
            {
                string txName = mat.DiffuseTexture;
                if (txName != null && File.Exists(inputDirectory + txName) && !texFilenames.Contains(txName))
                {
                    texFilenames.Add(txName);
                }
                txName = mat.SphereTexture;
                if (txName != null && File.Exists(inputDirectory + txName) && !texFilenames.Contains(txName))
                {
                    texFilenames.Add(txName);
                }
            }

            foreach (string txName in texFilenames)
            {
                uint hash = crc32.GetCRC32(inputDirectory + txName);
                inputFileHashes.Add(txName, hash);
            }

            foreach (PMXMaterial mat in pmxmd.Materials)
            {
                mat.DiffuseTexture = GetTargetFileName(inputFileHashes, outputFileHashes, texFiles, texDirectory, inputDirectory, mat.DiffuseTexture);
                mat.SphereTexture = GetTargetFileName(inputFileHashes, outputFileHashes, texFiles, texDirectory, inputDirectory, mat.SphereTexture);
            }

            foreach (PMXBone bn in pmxmd.Bones)
            {
                string bnNew = bn.NameJP;
                if (jpBones.ContainsKey(bnNew) || !bn.Visible)
                {
                    continue;
                }

                bool isInSlot = false;

                PMXDisplaySlot iksSlot = null;
                PMXDisplaySlot othSlot = null;

                foreach (PMXDisplaySlot slot in pmxmd.DisplaySlots)
                {
                    if (slot.NameEN == "IK")
                    {
                        iksSlot = slot;
                    }
                    if (slot.NameEN == "Others")
                    {
                        othSlot = slot;
                    }

                    foreach (PMXBasePart bp in slot.References)
                    {
                        if (bp == bn)
                        {
                            isInSlot = true;
                            break;
                        }
                    }

                    if (isInSlot)
                    {
                        break;
                    }
                }

                if (isInSlot)
                {
                    continue;
                }

                bn.NameEN = bnNew;

                PMXDisplaySlot ds = null;
                string slotName = null;

                if (bn.Translatable)
                {
                    ds = iksSlot;
                    slotName = "IK";
                } else
                {
                    ds = othSlot;
                    slotName = "Others";
                }

                if (ds == null)
                {
                    ds = new PMXDisplaySlot(pmxmd);
                    ds.NameEN = slotName;
                    ds.NameJP = slotName;
                    pmxmd.DisplaySlots.Add(ds);
                }
                ds.References.Add(bn);
            }

            pmxmd.SaveToFile(outname);

            Console.WriteLine("Success");
            System.Threading.Thread.Sleep(1500);
        }

        private static string GetTargetFileName(Dictionary<string, uint> inputFileHashes, Dictionary<uint, string> outputFileHashes, List<string> texFiles, string outputDirectory, string inputDirectory, string filename)
        {
            if (filename == null || !inputFileHashes.ContainsKey(filename))
            {
                return filename;
            }

            uint hash = inputFileHashes[filename];
            if (outputFileHashes.ContainsKey(hash))
            {
                return "tex" + Path.DirectorySeparatorChar + outputFileHashes[hash];
            }

            string ext = Path.GetExtension(inputDirectory + filename);

            int i = 1;
            while (texFiles.Contains("tx" + i.ToString()))
            {
                i++;
            }

            texFiles.Add("tx" + i.ToString());

            string outFn = "tx" + i.ToString() + ext;
            string outputCopy = outputDirectory + Path.DirectorySeparatorChar + "tx" + i.ToString() + ext;

            outputFileHashes.Add(hash, outFn);

            File.Copy(inputDirectory + filename, outputCopy, false);

            return "tex" + Path.DirectorySeparatorChar + outFn;
        }

        private static bool BoneHasAssignedVertices(PMXModel mdl, PMXBone bn, Dictionary<PMXBone, bool> boneHasDirectVertices)
        {
            if (!boneHasDirectVertices.ContainsKey(bn))
            {
                return false;
            }
            if (!mdl.Bones.Contains(bn))
            {
                return false;
            }

            if (bn.NameJP == "両目")
            {
                return true;
            }

            if (bn.NameJP.Length > 2 && bn.NameJP.Substring(bn.NameJP.Length - 2, 2) == "ＩＫ")
            {
                return true;
            }

            bool hasDirect = boneHasDirectVertices[bn];

            foreach (PMXBone bone in mdl.Bones)
            {
                if (hasDirect)
                {
                    break;
                }
                if (bone.Parent == bn)
                {
                    hasDirect |= BoneHasAssignedVertices(mdl, bone, boneHasDirectVertices);
                }
            }
            return hasDirect;
        }

        private static void CreateMergeMorph(PMXModel mdl, string newNameEn, string newNameJp, string[] joins)
        {
            if (joins.Length == 0)
            {
                return;
            }

            List<PMXMorph> morphs = new List<PMXMorph>();
            int lowestIndex = int.MaxValue;
            foreach (string j in joins)
            {
                PMXMorph found = null;
                int i = 0;
                foreach (PMXMorph m in mdl.Morphs)
                {
                    if (m.NameJP == j)
                    {
                        found = m;
                        if (i < lowestIndex)
                        {
                            lowestIndex = i;
                        }
                        break;
                    }
                    i++;
                }
                if (found == null)
                {
                    return;
                }
                morphs.Add(found);
            }

            PMXMorph n = new PMXMorph(mdl);
            n.NameEN = newNameEn;
            n.NameJP = newNameJp;
            n.Panel = morphs[0].Panel;

            foreach (PMXMorph m in morphs)
            {
                PMXMorphOffsetGroup mog = new PMXMorphOffsetGroup(mdl, n);
                mog.MorphTarget = m;
                mog.Strength = 1.0f;
                n.Offsets.Add(mog);
            }

            mdl.Morphs.Insert(lowestIndex, n);
        }

        private static List<PMXBone> FindNameSuitableChildren(PMXBone bn, string parsedName, int bnQueueIndex, PMXModel mdl)
        {
            PMXBone suitableChild = null;
            int childIndex = -1;

            foreach (PMXBone b in mdl.Bones)
            {
                if (b.Parent != bn)
                {
                    continue;
                }

                int offset = b.NameJP.ToLowerInvariant().IndexOf(parsedName.ToLowerInvariant());
                if (offset != -1)
                {
                    string remaining = b.NameJP.Substring(offset + parsedName.Length + 1);
                    int nextSub = remaining.IndexOf("_");
                    if (nextSub >= 0)
                    {
                        remaining = remaining.Substring(0, nextSub);
                    }

                    if (remaining == "root")
                    {
                        remaining = b.NameJP.Substring(offset + parsedName.Length + 6);
                        nextSub = remaining.IndexOf("_");
                        if (nextSub >= 0)
                        {
                            remaining = remaining.Substring(0, nextSub);
                        }
                    }

                    int testNext;
                    if (int.TryParse(remaining, out testNext) && testNext == bnQueueIndex + 1)
                    {
                        suitableChild = b;
                        childIndex = testNext;
                    }
                }
            }

            if (suitableChild != null)
            {
                List<PMXBone> bones = new List<PMXBone>() { suitableChild };
                List<PMXBone> addtl = FindNameSuitableChildren(suitableChild, parsedName, childIndex, mdl);
                if (addtl != null)
                {
                    bones.AddRange(addtl);
                }
                return bones;
            }

            return null;
        }

        private static string GenerateNewName(string parsedName)
        {
            string[] groups = parsedName.Split(new char[] { '_' });

            string res = "";

            foreach (string p in groups)
            {
                switch(p.ToLowerInvariant())
                {
                    case "side":
                        res += "S";
                        break;
                    case "front":
                        res += "F";
                        break;
                    case "center":
                        res += "C";
                        break;
                    case "back":
                        res += "B";
                        break;
                    case "left":
                        res += "L";
                        break;
                    case "right":
                        res += "R";
                        break;
                    case "neutral":
                        res += "N";
                        break;
                    default:
                        if (res != "") res += "_";
                        res += FirstLetterUpper(p);
                        break;
                }
            }

            return res;
        }

        private static string FirstLetterUpper(string input)
        {
            if(input == null || input == "")
            {
                return input;
            }
            
            char[] a = input.ToCharArray();
            a[0] = char.ToUpperInvariant(a[0]);
            return new string(a);
        }
    }
}
