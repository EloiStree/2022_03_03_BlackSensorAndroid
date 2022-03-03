using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Eloi;
public class PhoneSensorPushAndReceive : MonoBehaviour
{
    public MobileSensorUpdate m_mobileSensorUpdate;
    public int maxLeft=4;
    public int maxRight=4;

    [Header("Phone id")]
    public string phoneId = "D";

    [Header("To Track")]
    public string current_Acceleration;
    public string id_current_Acceleration = "📱a";
    public string current_CompassRaw;
    public string id_current_CompassRaw = "📱cr";
    public string current_CompassHeading;
    public string id_current_CompassHeading = "📱ch";
    public string current_Gyro;
    public string id_current_Gyro = "📱g";
    public string current_GPS;
    public string id_current_GPS = "📱gps";

    public string id_inputString = "📱is";

    [Header("To Push")]
    public Eloi.PrimitiveUnityEvent_String m_textToSentOnNetwork;
    [Header("Input String Received")]
    public Eloi.PrimitiveUnityEvent_String m_inputStringReceived;


    public StringClampHistory m_historyReceivedToUncompress;

    [System.Serializable]
    public class MobileSensorUpdate
    {
        public Vector3 m_acceleration;
        public Vector3 m_gyroGravity;
        public Vector3 m_compassRawVector;
        public Vector3 m_locationGPS;
        public float m_compassMagneticHeading;
    }

    public void Awake()
    {
        Input.compass.enabled = true;
        Input.location.Start();
        Input.gyro.enabled = true;

    }

    public void Push()
    {
        Vector3 location = new Vector3(
            Input.location.lastData.longitude,
            Input.location.lastData.latitude,
            Input.location.lastData.horizontalAccuracy
            );
        Eloi.E_StringCompression.Vector3ToString(Input.acceleration, in maxLeft, in maxRight, out current_Acceleration);
        Eloi.E_StringCompression.Vector3ToString(Input.gyro.gravity, in maxLeft, in maxRight, out current_Gyro);
        Eloi.E_StringCompression.Vector3ToString(Input.compass.rawVector, in maxLeft, in maxRight, out current_CompassRaw);
        Eloi.E_StringCompression.FloatToString(Input.compass.magneticHeading, in maxLeft, in maxRight, out current_CompassHeading);
        Eloi.E_StringCompression.Vector3ToString(location, in maxLeft, in maxRight, out current_GPS);

        m_textToSentOnNetwork.Invoke(id_current_Acceleration + ":" + current_Acceleration);
        m_textToSentOnNetwork.Invoke(id_current_CompassRaw + ":" + current_CompassRaw);
        m_textToSentOnNetwork.Invoke(id_current_CompassHeading + ":" + current_CompassHeading);
        m_textToSentOnNetwork.Invoke(id_current_Gyro + ":" + current_Gyro);
        m_textToSentOnNetwork.Invoke(id_current_GPS + ":" + current_GPS);
    }
    private void Update()
    {
        if (Input.inputString.Length > 0)
            m_textToSentOnNetwork.Invoke(id_inputString + ":" + id_inputString);
    }

    public void ReceiveMessageToUncompress(string text)
    {
        m_historyReceivedToUncompress.PushIn(text);
        if (text.IndexOf(id_current_Acceleration) >= 0)
        {
            text = text.Replace(id_current_Acceleration + ":", "");
            Eloi.E_StringCompression.StringToVector3(in text,
                out m_mobileSensorUpdate.m_acceleration);
        }
        else  if (text.IndexOf(id_current_Gyro) >= 0)
        {
            text = text.Replace(id_current_Gyro + ":", "");
            Eloi.E_StringCompression.StringToVector3(in text,
                out m_mobileSensorUpdate.m_gyroGravity);
        }
        else if (text.IndexOf(id_current_CompassRaw) >= 0)
        {
            text = text.Replace(id_current_CompassRaw + ":", "");
            Eloi.E_StringCompression.StringToVector3(in text,
                out m_mobileSensorUpdate.m_compassRawVector);
        }
        else if (text.IndexOf(id_current_GPS) >= 0)
        {
            text = text.Replace(id_current_GPS + ":", "");
            Eloi.E_StringCompression.StringToVector3(in text,
                out m_mobileSensorUpdate.m_locationGPS);
        }
        else if (text.IndexOf(id_current_CompassHeading) >= 0)
        {
            text = text.Replace(id_current_CompassHeading + ":", "");
            Eloi.E_StringCompression.StringToFloat(in text,
                out m_mobileSensorUpdate.m_compassMagneticHeading);
        }
        else if (text.IndexOf(id_inputString) >= 0)
        {
            text = text.Replace(id_inputString + ":", "");
            m_inputStringReceived.Invoke(text);
        }
    }
}
