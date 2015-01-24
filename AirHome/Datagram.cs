﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AirHome
{
    /// <summary>
    /// 报文结构体
    /// </summary>
    public struct Datagram
    {
        /// <summary>
        /// 起始符
        ///     <para>只读属性</para>
        ///     <para>值为0X02</para>
        /// </summary>
        public Byte Stx
        {
            get
            {
                return 0X02;
            }
            private set { }
        }

        /// <summary>
        /// 消息头
        ///     <para>长度为16字节</para>
        /// </summary>
        public MessageHead Head { get; set; }

        /// <summary>
        /// 消息体
        ///     <para>长度可变</para>
        /// </summary>
        public MessageBody Body { get; set; }

        /// <summary>
        /// 结束符
        ///     <para>只读属性</para>
        ///     <para>值为0X03</para>
        /// </summary>
        public Byte Etx
        {
            get
            {
                return 0X03;
            }
            private set { }
        }

        /// <summary>
        /// 获取消息报文字节数组
        /// </summary>
        /// <returns></returns>
        public Byte[] GetDatagram()
        {
            List<Byte> dg = new List<byte>();
            dg.Add(this.Stx);
            dg.AddRange(this.Head.GetHead());
            dg.AddRange(this.Body.GetBody());
            dg.Add(this.Etx);

            return dg.ToArray();
        }

        /// <summary>
        /// 获取消息报文十六进制字符串
        /// </summary>
        /// <param name="separator">
        /// 分隔符
        ///     <para>默认为空字符</para>
        /// </param>
        /// <returns></returns>
        public string ToHexString(string separator = "")
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in this.GetDatagram())
            {
                sb.Append(item.ToString("X2") + separator);
            }

            return sb.ToString().TrimEnd(separator.ToCharArray());
        }

        /// <summary>
        /// 获取消息报文字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.GetDatagram());
        }
    }

    /// <summary>
    /// 消息头结构体
    /// </summary>
    public struct MessageHead
    {
        /// <summary>
        /// 消息体长度
        ///     <para>UInt16类型，长度为2个字节</para>
        /// </summary>
        public UInt16 Length { get; set; }

        /// <summary>
        /// 消息类型
        ///     <para>Byte类型，长度为1个字节</para>
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// 消息序号
        ///     <para>UInt32类型，长度为4个字节</para>
        /// </summary>
        public UInt32 SeqNumber { get; set; }

        /// <summary>
        /// 预留字段
        ///     <para>UInt64类型，长度为7字节</para>
        /// </summary>
        public UInt64 Reserved { get; set; }

        /// <summary>
        /// 消息体CRC校验
        ///     <para>UInt16类型，长度为2个字节</para>
        /// </summary>
        public UInt16 Crc { get; set; }

        /// <summary>
        /// 获取消息头字节数组
        /// </summary>
        /// <returns></returns>
        public Byte[] GetHead()
        {
            List<Byte> mh = new List<byte>();
            mh.Add((Byte)(this.Length >> 8));
            mh.Add((Byte)(this.Length));

            mh.Add((Byte)(this.Type));

            for (int i = 24; i >= 0; i -= 8)
            {
                mh.Add((Byte)(this.SeqNumber >> i));
            }

            for (int j = 0; j < 7; j++)
            {
                mh.Add(0X00);
            }

            mh.Add((Byte)(this.Crc >> 8));
            mh.Add((Byte)(this.Crc));

            return mh.ToArray();
        }

        /// <summary>
        /// 获取消息头十六进制字符串
        /// </summary>
        /// <param name="separator">
        /// 分隔符
        ///     <para>默认为空字符</para>
        /// </param>
        /// <returns></returns>
        public string ToHexString(string separator = "")
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in this.GetHead())
            {
                sb.Append(item.ToString("X2") + separator);
            }

            return sb.ToString().TrimEnd(separator.ToCharArray());
        }

        /// <summary>
        /// 获取消息头字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.GetHead());
        }
    }

    /// <summary>
    /// 消息体结构体
    /// </summary>
    public struct MessageBody
    {
        /// <summary>
        /// 消息ID
        ///     <para>UInt16类型，长度为2个字节</para>
        /// </summary>
        public MessageId MsgId { get; set; }

        /// <summary>
        /// 设备ID
        ///     <para>UInt64类型，长度为2个字节</para>
        /// </summary>
        public UInt64 DevId { get; set; }

        /// <summary>
        /// 参数列表
        ///     <para>长度可变</para>
        /// </summary>
        public List<Parameter> PmtList { get; set; }

        /// <summary>
        /// 获取消息体字节数组
        /// </summary>
        /// <returns></returns>
        public Byte[] GetBody()
        {
            List<Byte> mb = new List<byte>();
            mb.Add((Byte)((UInt16)(this.MsgId) >> 8));
            mb.Add((Byte)(this.MsgId));

            for (int i = 56; i >= 0; i -= 8)
            {
                mb.Add((Byte)(this.DevId >> i));
            }

            foreach (var pmt in this.PmtList)
            {
                mb.AddRange(pmt.GetParameter());
            }

            return mb.ToArray();
        }

        /// <summary>
        /// 获取消息体十六进制字符串
        /// </summary>
        /// <param name="separator">
        /// 分隔符
        ///     <para>默认为空字符</para>
        /// </param>
        /// <returns></returns>
        public string ToHexString(string separator = "")
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in this.GetBody())
            {
                sb.Append(item.ToString("X2") + separator);
            }

            return sb.ToString().TrimEnd(separator.ToCharArray());
        }

        /// <summary>
        /// 获取消息体字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Encoding.UTF8.GetString(this.GetBody());
        }
    }

    /// <summary>
    /// 参数结构体
    /// </summary>
    public struct Parameter
    {
        /// <summary>
        /// 参数类型
        ///     <para>Byte类型，长度为1个字节</para>
        /// </summary>
        public ParameterType Type { get; set; }

        /// <summary>
        /// 参数值
        ///     <para>长度可变，十六进制表示</para>
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// 参数结束符
        ///     <para>只读属性</para>
        ///     <para>值为0X00</para>
        /// </summary>
        public Byte End
        {
            get
            {
                return 0X00;
            }
            private set { }
        }

        /// <summary>
        /// 获取参数字节数组
        /// </summary>
        /// <returns></returns>
        public Byte[] GetParameter()
        {
            List<Byte> pmt = new List<byte>();
            pmt.Add((Byte)(this.Type));
            pmt.AddRange(Encoding.UTF8.GetBytes(this.Value));
            pmt.Add(this.End);

            return pmt.ToArray();
        }
    }

    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 服务器到设备
        /// </summary>
        ServerToDevice = 0X00,

        /// <summary>
        /// 设备到服务器
        /// </summary>
        DeviceToServer = 0X01,

        /// <summary>
        /// 设备主动上报
        /// </summary>
        DeviceReport = 0X02,

        /// <summary>
        /// 服务器响应设备上报
        /// </summary>
        ServerResponse = 0X03
    }

    /// <summary>
    /// 消息ID枚举
    /// </summary>
    public enum MessageId
    {
        /// <summary>
        /// 包含多个功能
        /// </summary>
        Multifunction = 0X0000,

        /// <summary>
        /// 搜索设备
        /// </summary>
        SearchDevice = 0X0001,

        /// <summary>
        /// 定位设备
        /// </summary>
        LocateDevice = 0X0002,

        /// <summary>
        /// 设备分区
        /// </summary>
        DevicePartition = 0X0003,

        /// <summary>
        /// 设置名称
        /// </summary>
        SettingName = 0X0004,

        /// <summary>
        /// 设置描述
        /// </summary>
        SettingDescription = 0X0005,

        /// <summary>
        /// 打开或关闭设备
        /// </summary>
        Switch = 0X1000,

        /// <summary>
        /// 调节亮度
        /// </summary>
        Brightness = 0X1001,

        /// <summary>
        /// 调节色温
        /// </summary>
        ColorTemperature = 0X1002,

        /// <summary>
        /// 调节RGB
        /// </summary>
        AdjustRgb = 0X1003
    }

    /// <summary>
    /// 参数类型枚举
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// 无参数
        /// </summary>
        None = 0X00,

        /// <summary>
        /// 回路数量
        /// </summary>
        CircuitCount = 0X01,

        /// <summary>
        /// 回路编号
        /// </summary>
        CircuitNo = 0X02,

        /// <summary>
        /// 分区编号
        /// </summary>
        PartitionNo = 0X03,

        /// <summary>
        /// 设备功能
        /// </summary>
        DeviceFunction = 0X04,

        /// <summary>
        /// 设备名称
        /// </summary>
        DeviceName = 0X05,

        /// <summary>
        /// 描述信息
        /// </summary>
        DeviceDescription = 0X06,

        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime = 0X07,

        /// <summary>
        /// SSID
        /// </summary>
        Ssid = 0X08,

        /// <summary>
        /// WiFi密码
        /// </summary>
        WifiPassword = 0X09,

        /// <summary>
        /// 开关
        /// </summary>
        Switch = 0X10,

        /// <summary>
        /// 亮度
        /// </summary>
        Brightness = 0X11,

        /// <summary>
        /// 色温
        /// </summary>
        ColorTemperature = 0X12,

        /// <summary>
        /// RGB
        /// </summary>
        Rgb = 0X13,

        /// <summary>
        /// 响应码
        /// </summary>
        ResponseCode = 0XFF
    }

    /// <summary>
    /// 设备功能枚举
    /// </summary>
    public enum DeviceFunction
    {
        /// <summary>
        /// 定时功能
        /// </summary>
        Timing = 0X00,

        /// <summary>
        /// 灯具开关
        /// </summary>
        LightSwitch = 0X10,

        /// <summary>
        /// 亮度调节功能
        /// </summary>
        Brightness = 0X11,

        /// <summary>
        /// 色温调节功能
        /// </summary>
        ColorTemperature = 0X12,

        /// <summary>
        /// RGB调节功能
        /// </summary>
        Rgb = 0X13,

        /// <summary>
        /// 摄像头开关
        /// </summary>
        CameraSwitch = 0X20,

        /// <summary>
        /// 拍照功能
        /// </summary>
        Photograph = 0X21,

        /// <summary>
        /// 录像功能
        /// </summary>
        Video = 0X22,

        /// <summary>
        /// 温度传感器
        /// </summary>
        TemperatureSensor = 0X50,

        /// <summary>
        /// 湿度传感器
        /// </summary>
        HumiditySensor = 0X51,

        /// <summary>
        /// PM2.5传感器
        /// </summary>
        Pm25 = 0X55
    }

    /// <summary>
    /// 响应码
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed = 0X00,

        /// <summary>
        /// 命令解析错误
        /// </summary>
        AnalysisError = 0X01,

        /// <summary>
        /// 消息体CRC校验错误
        /// </summary>
        CrcError = 0X02,

        /// <summary>
        /// 不支持该命令
        /// </summary>
        Nonsupport = 0X03,

        /// <summary>
        /// 命令无法执行
        /// </summary>
        NotExecuted = 0X04,

        /// <summary>
        /// 参数个数错误
        /// </summary>
        ParameterCountError = 0X05,

        /// <summary>
        /// 参数格式错误
        /// </summary>
        ParameterFormatError = 0X06,

        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown = 0XFF,
    }
}