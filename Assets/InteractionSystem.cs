using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float rayLength = 3f; // Distanța maximă de interacțiune
    public LayerMask interactionLayer; // Layer-ul pentru obiecte interactibile
    public Camera playerCamera; // Referință la camera principală
    
    private GameObject currentTarget; // Obiectul curent țintit

    private bool isItemHeld = false;

    public GameObject InteractionText;

    public GameObject ItemContainer;

    public GameObject Flashlight;
    private bool isFlashlightActive = false;

    private GameObject Item;

    public GameObject InteractionCircle;
    
    void Start()
    {
        // Găsește toate obiectele cu tag-ul "Item" și setează isKinematic la true
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach(GameObject item in items)
        {
            if(item.GetComponent<Rigidbody>() != null)
            {
                item.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
        Flashlight.SetActive(false);
        isFlashlightActive = false;
    }

    void Update()
    {
        InteractionTextSystem();
        FlashlightSystem();

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isItemHeld)
        {
            Drop();
        }
    }
    
    void InteractionTextSystem()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        // Debug ray pentru vizualizare în editor
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow);
        
        if (Physics.Raycast(ray, out hit, rayLength, interactionLayer))
        {
            // Verificăm dacă obiectul are tag-ul "Item"
            if (hit.collider.CompareTag("Item"))
            {
                currentTarget = hit.collider.gameObject;
                InteractionText.SetActive(true);
                InteractionText.GetComponent<TMPro.TextMeshProUGUI>().text = currentTarget.name;
                InteractionCircle.SetActive(true);
            }
        }
        else
        {
            InteractionText.SetActive(false);
            currentTarget = null;
            InteractionText.GetComponent<TMPro.TextMeshProUGUI>().text = " ";
            InteractionCircle.SetActive(false);
        }
    }
    
    
    void PickUp()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, rayLength, interactionLayer))
        {
            if (hit.collider.CompareTag("Item") && !isItemHeld)
            {
                // Stocăm referința Rigidbody pentru eficiență
                Rigidbody itemRb = hit.collider.gameObject.GetComponent<Rigidbody>();
                if (itemRb != null)
                {
                    hit.collider.gameObject.transform.position = ItemContainer.transform.position;
                    hit.collider.gameObject.transform.rotation = ItemContainer.transform.rotation;
                    hit.collider.gameObject.transform.SetParent(ItemContainer.transform);
                    isItemHeld = true;
                    Item = hit.collider.gameObject;
                    itemRb.useGravity = false;
                    itemRb.isKinematic = true;
                }
            }
        }
    }

    void Drop()
    {
        if (isItemHeld == true && Item != null)
        {
            Item.GetComponent<Rigidbody>().useGravity = true;
            Item.GetComponent<Rigidbody>().isKinematic = false;
            Item.GetComponent<Rigidbody>().AddForce(playerCamera.transform.forward * 200);
            Item.GetComponent<Rigidbody>().AddForce(playerCamera.transform.up * 100);
            Item.transform.SetParent(null);
            isItemHeld = false;
            Item = null;
        }
    }

    void FlashlightSystem()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F key pressed");
            isFlashlightActive = !isFlashlightActive;
            Debug.Log($"Flashlight active: {isFlashlightActive}");
            Flashlight.SetActive(isFlashlightActive);
        }
    }
    
}

