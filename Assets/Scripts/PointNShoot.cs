﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PointNShoot : MonoBehaviour
{
    public AK.Wwise.Event sndLaser;

    public GameObject wwiseObj;

    [SerializeField] private Texture2D reticule;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Camera cam;
    [SerializeField] private Image cooldownOverlay, cursor;
    [SerializeField] private GameObject redCross;

    private Vector2 mousePosition = Vector2.zero;
    private float cursorIconWidth, cursorIconHeight, cooldownTime = 0.2f;
    private bool isOnCooldown = false, doCooldown = false;

    private bool inWaitASecCor = false;

    void Start()
    {
        Cursor.visible = false;
        cursorIconHeight = reticule.height;
        cursorIconWidth = reticule.width;
        cooldownOverlay.gameObject.SetActive(false);

        redCross.SetActive(false);
        if(SceneManager.GetActiveScene().name == "Level" || SceneManager.GetActiveScene().name == "Level_Miled")
        {
            doCooldown = true;
        }
        

    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        cursor.transform.position = new Vector2(mousePosition.x, Input.mousePosition.y);
        cooldownOverlay.transform.position = new Vector2(mousePosition.x, Input.mousePosition.y);
        redCross.transform.position = new Vector2(mousePosition.x, Input.mousePosition.y);

        if (Input.GetButtonDown("Fire1") && isOnCooldown && !inWaitASecCor)
        {
            StartCoroutine(WaitASec(0.2f));
        }        

        if (Input.GetButtonDown("Fire1") && !isOnCooldown)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit RayHit;
            if (Physics.Raycast(ray, out RayHit))
            {
                sndLaser.Post(wwiseObj);
                GameObject laser = Instantiate(laserPrefab);
                laser.transform.position = cam.transform.position + new Vector3(0, 0, 1);

                Vector3 targetPos = RayHit.point;
                LaserProjectile lp = laser.GetComponent<LaserProjectile>();
                lp.SetTarget(targetPos);

                Vector3 direction = targetPos - cam.transform.position;
                Vector3 startRotation = laser.transform.eulerAngles;
                laser.transform.rotation = Quaternion.LookRotation(direction);
                laser.transform.eulerAngles += startRotation;

                if (doCooldown)
                {
                    StartCoroutine(DoCooldown());
                }
            }
        }       
    }

    private IEnumerator WaitASec(float time)
    {
        redCross.SetActive(true);
        yield return new WaitForSeconds(time);
        redCross.SetActive(false);
    }

    private IEnumerator DoCooldown()
    {
        isOnCooldown = true;
        cooldownOverlay.gameObject.SetActive(true);
        float startTime = Time.time;
        float progress = 0;
        RectTransform rt = cooldownOverlay.GetComponent<RectTransform>();
        while (progress < 1.0f)
        {
            progress = Mathf.Lerp(0, 1, (Time.time - startTime) / cooldownTime);
            float valueToInterpolate = Mathf.Lerp(150, 0, (Time.time - startTime) / cooldownTime);
            rt.sizeDelta = new Vector2(valueToInterpolate, valueToInterpolate);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        cooldownTime += 0.1f;
        isOnCooldown = false;
        cooldownOverlay.gameObject.SetActive(false);
    }

}
