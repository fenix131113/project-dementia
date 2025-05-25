using UnityEngine;
using TMPro;

namespace Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshPro))]
    public class CurvedTextMeshPro : MonoBehaviour
    {
        [SerializeField] private float curveRadius = 2f;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (!_text || _text.textInfo.characterCount == 0)
                return;

            _text.ForceMeshUpdate();

            var textInfo = _text.textInfo;

            for (var i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                var vertices = meshInfo.vertices;

                for (var c = 0; c < textInfo.characterCount; c++)
                {
                    var charInfo = textInfo.characterInfo[c];

                    if (!charInfo.isVisible || charInfo.materialReferenceIndex != i)
                        continue;

                    var vertexIndex = charInfo.vertexIndex;

                    for (var j = 0; j < 4; j++)
                    {
                        var orig = vertices[vertexIndex + j];

                        var angle = orig.x / curveRadius;
                        var x = Mathf.Sin(angle) * curveRadius;
                        var z = curveRadius - Mathf.Cos(angle) * curveRadius;

                        vertices[vertexIndex + j] = new Vector3(x, orig.y, z);
                    }
                }

                meshInfo.mesh.vertices = vertices;
                _text.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}