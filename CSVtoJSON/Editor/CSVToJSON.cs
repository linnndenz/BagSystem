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

        //创建目标文件夹
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        TextAsset[] textAssets = Selection.GetFiltered<TextAsset>(SelectionMode.Assets);//原始csv文件

        if (textAssets.Length <= 0 || textAssets == null) {
            Debug.Log("未选中文件");
            return;
        }

        for (int i = 0; i < textAssets.Length; i++) {
            if (!File.Exists(path + textAssets[i].name + ".json")) {
                //创建json文件
                StreamWriter fileWriter = new StreamWriter(File.Create(path + textAssets[i].name + ".json"));

                //读取每个csv文件内容
                string[] context = textAssets[i].text.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (context.Length <= 1 || context == null) {
                    Debug.Log("文件为空： " + textAssets[i].name);
                    failCnt++;
                    continue;
                }

                //读取每个cvs的标头
                string[] titles = context[0].Split(',');

                //写入格式
                string data = "";
                data += "{" + "\n";
                data += "  \"list\": [" + "\n";

                //遍历物品
                for (int itemCnt = 1; itemCnt < context.Length; itemCnt++) {
                    string[] info = context[itemCnt].Split(',');
                    data += "    {" + "\n";
                    //遍历单个物品信息
                    for (int titleCnt = 0; titleCnt < titles.Length; titleCnt++) {
                        if (titleCnt != titles.Length - 1) {
                            data += "      " + "\"" + titles[titleCnt].ToString() + "\": " + info[titleCnt].ToString() + "," + "\n";
                        } else {
                            data += "      " + "\"" + titles[titleCnt].ToString() + "\": " + info[titleCnt].ToString() + "\n";//没有后续不能加逗号
                        }
                    }
                    if (itemCnt != context.Length - 1) {
                        data += "    }" + "," + "\n";
                    } else {
                        data += "    }" + "\n";//没有后续不能加逗号
                    }
                }

                //写入格式
                data += "  ]" + "\n";
                data += "}";

                //转进json文件
                JsonUtility.ToJson(data);


                fileWriter.Write(data);
                fileWriter.Close();
                fileWriter.Dispose();

                successCnt++;

            } else {
                Debug.Log("重名文件： " + textAssets[i].name);
                failCnt++;
            }
        }

        AssetDatabase.Refresh();//刷新资源窗口界面
        Debug.Log("转化完成。 " + "成功： " + successCnt.ToString() + " 失败： " + failCnt.ToString());
    }
}
