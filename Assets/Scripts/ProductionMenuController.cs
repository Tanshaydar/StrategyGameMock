using UnityEngine;
using UnityEngine.UI;

public class ProductionMenuController : MonoBehaviour
{
    public InfiniteScroll scroll;
    private int count = 100000;

    void Start()
    {
        scroll.FillItem += (int index, GameObject item) =>
        {
            // here we can fill and modify item prefab
            // change text, image, ect
            // by index we can get data from JSON array, i.g.
            // item.transform.GetChild(0).GetComponent<Text>().text = "item #" + index;
            item.transform.GetChild(0).GetComponentInChildren<Text>().text = "Barrack #" + index;
            item.transform.GetChild(1).GetComponentInChildren<Text>().text = "PowerPlant #" + index;
        };

        scroll.PullLoad += (InfiniteScroll.Direction obj) =>
        {
            // here we listen pull-to-refresh event and process it
            // it could be loading data from server to JSON object and append to list
            // to do it, call ApplyDataTo function, where arg1 = common items count after appending, arg2 = count to append, arg3 = direction to append (top or bottom)
            count += 20;
            scroll.ApplyDataTo(count, 20, obj);
        };

        // function to initialize infinite scroll
        scroll.InitData(count);
    }
}