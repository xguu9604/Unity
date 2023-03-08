using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ������ ���� ����
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    // UI�� ���� ����
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    public Text maxScoreText;
    public Text scoreText;
    public Text stageText;
    public Text playTimeText;
    public Text playHealthText;
    public Text playAmmoText;
    public Text playCoinText;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyAText;
    public Text enemyBText;
    public Text enemyCText;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreText;
    public Text bestScoreText;


    void Awake()
    {
        enemyList = new List<int>();
        maxScoreText.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        gamePanel.SetActive(false);
        curScoreText.text = scoreText.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore)
        {
            bestScoreText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
    }


    public void Restart()
    {
        SceneManager.LoadScene(0);
    }


    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        enemyCntA = 0;
        enemyCntB = 0;
        enemyCntC = 0;
        enemyCntD = 0;

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);

        isBattle = false;
        stage++;
    }


    IEnumerator InBattle()
    {
        if (stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEmemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
            HitBox enemy = instantEmemy.GetComponent<HitBox>();
            enemy.target = player.transform;
            enemy.gameManager = this;
            boss = instantEmemy.GetComponent<Boss>();
        } 
        else
        {
            for (int index = 0; index < stage; index++)
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }

            while (enemyList.Count > 0)
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
                // ������ȭ �� ģ������ ���� ȭ�鿡 �ö�� ģ������ ������ �� �� ����
                // ��ũ��Ʈ�� ��ȯ�� �� �ش� Ÿ�ٵ��� ���� ��������� �Ѵ�!
                HitBox enemy = instantEnemy.GetComponent<HitBox>();
                enemy.target = player.transform;
                enemy.gameManager = this;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(2f);
            }
        }

        // ���Ͱ� ���� óġ �ɶ����� �׳� ������ ������ Ȯ��
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
        {
            yield return null;
        }

        // ���� while ���� ���������� ���͸� ���� ��Ҵٴ� �ǹ�
        // 4�ʵڿ� �������� �Ϸ�
        yield return new WaitForSeconds(4f);
        boss = null;
        StageEnd();
    }

    private void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }


    // update()�� ���� �� ȣ��Ǵ� �����ֱ�
    void LateUpdate()
    {
        // ��� UI
        scoreText.text = string.Format("{0:n0}", player.score);
        stageText.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);

        playTimeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec);
        
        // �÷��̾� UI
        playHealthText.text = player.health + " / " + player.maxHealth;
        playCoinText.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null)
            playAmmoText.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playAmmoText.text = "- / " + player.ammo;
        else
            playAmmoText.text = player.equipWeapon.currentAmmo + " / " + player.ammo;

        // ���� UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        // ���� ���� UI
        enemyAText.text = enemyCntA.ToString();
        enemyBText.text = enemyCntB.ToString();
        enemyCText.text = enemyCntC.ToString();

        // ���� ü�¹� UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 300;
        }
    }
}
