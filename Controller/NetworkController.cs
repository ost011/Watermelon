using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Threading.Tasks;

public class NetworkController
{
    public static NetworkController Instance = null;

    static NetworkController()
    {
        Instance = new NetworkController();
    }

    private NetworkController()
    {
#if UNITY_EDITOR

#else
        // ListenNetworkAvailable();
#endif
    }

    private const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
    private const string NCSI_TEST_RESULT = "Microsoft NCSI";
    private const string NCSI_DNS = "dns.msftncsi.com";
    private const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

    public bool IsInternetConnected()
    {
        try
        {
            // check ncsi test link
            var webClient = new WebClient();

            string result = webClient.DownloadString(NCSI_TEST_URL);

            if (result != NCSI_TEST_RESULT)
            {
                webClient.Dispose();

                return false;
            }

            //check ncsi dns ip
            var dnsHost = Dns.GetHostEntry(NCSI_DNS);
            if (dnsHost.AddressList.Length < 0 || !dnsHost.AddressList[0].ToString().Equals(NCSI_DNS_IP_ADDRESS))
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }

        // Debug.Log("Network Check~!! , True!");
        return true;
    }

    public void CheckInternetConnected(Action ifConnectionOK = null, Action whenNoConnectionDoNextStep = null)
    {
        var isConnected = true;

        try
        {
            CustomDebug.Log("CheckInternetConnected 1");

            // check ncsi test link
            var webClient = new WebClient();

            string result = webClient.DownloadString(NCSI_TEST_URL);

            if (result != NCSI_TEST_RESULT)
            {
                isConnected = false;

                webClient.Dispose();

                return;
            }

            CustomDebug.Log("CheckInternetConnected 2");

            //check ncsi dns ip
            var dnsHost = Dns.GetHostEntry(NCSI_DNS);

            if (dnsHost.AddressList.Length < 0 || !dnsHost.AddressList[0].ToString().Equals(NCSI_DNS_IP_ADDRESS))
            {
                isConnected = false;

                return;
            }
        }
        catch (Exception ex)
        {
            CustomDebug.Log($"check internet connecting Exception : {ex}");

            isConnected = false;
        }
        finally
        {
            CustomDebug.Log($"CheckInternetConnected finally ,isConnected : {isConnected} ");

            if (isConnected)
            {
                ifConnectionOK?.Invoke();
            }
            else
            {
                whenNoConnectionDoNextStep?.Invoke();
            }
        }
    }

    private void ListenNetworkAvailable()
    {
        NetworkChange.NetworkAvailabilityChanged +=
            new NetworkAvailabilityChangedEventHandler(NetworkAvailabilityChanged);

        // SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SessionSwithed);
    }

    private void NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs ne)
    {
        if (ne.IsAvailable)
        {
            CustomDebug.Log("Network ÀÌ µ¹¾Æ¿È~~~~~");
        }
        else
        {
            CustomDebug.Log("Network ÀÌ ´Ù¿î µÊ  !!!!!");
        }
    }
}
