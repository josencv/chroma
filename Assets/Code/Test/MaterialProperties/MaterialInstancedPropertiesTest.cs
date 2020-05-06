using UnityEngine;

namespace Chroma.Test.MaterialProperties
{
    public class MaterialInstancedPropertiesTest : MonoBehaviour
    {
        private MaterialObject[] objects;
        
        [SerializeField]
        private bool instanced = false;

        private void Awake()
        {
            objects = FindObjectsOfType<MaterialObject>();    
        }

        private void Start()
        {

            foreach(MaterialObject obj in objects)
            {
                Color color = Random.ColorHSV();
                if(instanced)
                {
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    obj.GetComponent<Renderer>().GetPropertyBlock(block);
                    block.SetColor("_Tint_test", color);
                    obj.GetComponent<Renderer>().SetPropertyBlock(block);
                }
                else
                {
                    obj.GetComponent<Renderer>().material.SetColor("_Tint_test", color);
                }
            }
        }
    }
}
