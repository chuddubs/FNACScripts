using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cacafier : Singletroon<Cacafier>
{
    [Serializable]
    public struct MatsToSwitch
    {
        [SerializeField]
        public int index;
        [SerializeField]
        public Material baseMat;
        [SerializeField]
        public Material cacaMat;
    }

    [Serializable]
    public class ModelToSwitch
    {
        public MeshRenderer meshRdr;
        public List<MatsToSwitch> matsToSwitches;

        public void SwitchMats(bool on)
        {
            Material[] materials = meshRdr.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                foreach (MatsToSwitch mat in matsToSwitches)
                {
                    if (i == mat.index)
                        materials[mat.index] = on ? mat.cacaMat : mat.baseMat;
                }
            }
            meshRdr.materials = materials;
        }
    };

    [Serializable]
    public class SkinnedModelToSwitch
    {
        public SkinnedMeshRenderer skinnedMeshRdr;
        public List<MatsToSwitch> matsToSwitches;

        public void SwitchMats(bool on)
        {
            Material[] materials = skinnedMeshRdr.materials;
            for (int i = 0; i > materials.Length; i++)
            {
                foreach (MatsToSwitch mat in matsToSwitches)
                {
                    if (i == mat.index)
                        materials[mat.index] = on ? mat.cacaMat : mat.baseMat;
                }
            }
            skinnedMeshRdr.materials = materials;
        }
    }

    public GameObject[] props;
    public ModelToSwitch[] models;
    public SkinnedModelToSwitch[] skModels;

    public void ToggleCaca(bool on)
    {
        foreach (GameObject p in props)
        {
            p.transform.GetChild(0).gameObject.SetActive(!on);
            p.transform.GetChild(1).gameObject.SetActive(on);
        }

        foreach(ModelToSwitch model in models)
        {
            model.SwitchMats(on);
        }

        foreach (SkinnedModelToSwitch skModel in skModels)
        {
            skModel.SwitchMats(on);
        }
    }
}
