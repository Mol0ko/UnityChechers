using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour
    {
        //Меш игрового объекта
        private MeshRenderer _mesh;
        //Список материалов на меше объекта
        private Material[] _meshMaterials = new Material[3];

        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        private ColorType _color;

        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;

        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        public BaseClickComponent Pair { get; set; }



        /// <summary>
        /// Добавляет дополнительный материал
        /// </summary>
        public void AddAdditionalMaterial(Material material, int index = 1)
        {
            if (index < 1 || index > 2)
            {
                Debug.LogError("Попытка добавить лишний материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = material;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Удаляет дополнительный материал
        /// </summary>
        public void RemoveAdditionalMaterial(int index = 1)
        {
            if (index < 1 || index > 2 || _meshMaterials.Count() >= index)
            {
                Debug.LogWarning("Попытка удалить несуществующий материал. Индекс может быть равен только 1 или 2");
                return;
            }
            _meshMaterials[index] = null;
            _mesh.materials = _meshMaterials.Where(t => t != null).ToArray();
        }

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;


        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter();

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit();

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick()
        {
            OnClickEventHandler?.Invoke(this);
        }

        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
        }

        protected virtual void Start()
        {
            _mesh = GetComponent<MeshRenderer>();
            //Этот список будет использоваться для набора материалов у меша,
            //в данном ДЗ достаточно массива из 3 элементов
            //1 элемент - родной материал меша, он не меняется
            //2 элемент - материал при наведении курсора на клетку/выборе фишки
            //3 элемент - материал клетки, на которую можно передвинуть фишку
            _meshMaterials[0] = _mesh.material;
        }
    }

    public enum ColorType: byte
    {
        White = 0,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}