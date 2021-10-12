using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSVtoJSON
{

    [MenuItem("Tools/CSVtoJSON")]
    public static void ChangeCsvToJson()
    {
        string path = Application.dataPath + "/Plugins/CSVtoJSON/Json/";

        int successCnt = 0;
        int failCnt = 0;

        //����Ŀ���ļ���
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.Assets);//ԭʼcsv�ļ�

        if (textAssets.Length <= 0 || textAssets == null) {
            Debug.Log("δѡ���ļ�");
            return;
        }

        for (int i = 0; i < textAssets.Length; i++) {
            if (!File.Exists(path + textAssets[i].name + ".json")) {
                //����json�ļ�
                StreamWriter fileWriter = new StreamWriter(File.Create(path + textAssets[i].name + ".json"));

                //��ȡÿ��csv�ļ�����
                string[] context = textAssets[i].text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (context.Length <= 1 || context == null) {
                    Debug.Log("�ļ�Ϊ�գ� " + textAssets[i].name);
                    failCnt++;
                    continue;
                }

                //��ȡÿ��cvs�ı�ͷ
                string[] titles = context[0].Split(',');

                //д���ʽ
                string data = "";
                data += "{" + "\n";
                data += "  \"list\": [" + "\n";

                //������Ʒ
                for (int itemCnt = 1; itemCnt < context.Length; itemCnt++) {
                    string[] info = context[itemCnt].Split(',');
                    data += "    {" + "\n";
                    //����������Ʒ��Ϣ
                    for (int titleCnt = 0; titleCnt < titles.Length; titleCnt++) {
                        if (titleCnt != titles.Length - 1) {
                            data += "      " + "\"" + titles[titleCnt].ToString() + "\": " + info[titleCnt].ToString() + "," + "\n";
                        } else {
                            data += "      " + "\"" + titles[titleCnt].ToString() + "\": " + info[titleCnt].ToString() + "\n";//û�к������ܼӶ���
                        }
                    }
                    if (itemCnt != context.Length - 1) {
                        data += "    }" + "," + "\n";
                    } else {
                        data += "    }" + "\n";//û�к������ܼӶ���
                    }
                }

                //д���ʽ
                data += "  ]" + "\n";
                data += "}";

                //ת��json�ļ�
                JsonUtility.ToJson(data);


                fileWriter.Write(data);
                fileWriter.Close();
                fileWriter.Dispose();

                successCnt++;

            } else {
                Debug.Log("�����ļ��� " + textAssets[i].name);
                failCnt++;
            }
        }

        AssetDatabase.Refresh();//ˢ����Դ���ڽ���
        Debug.Log("ת����ɡ� " + "�ɹ��� " + successCnt.ToString() + " ʧ�ܣ� " + failCnt.ToString());
    }
}
