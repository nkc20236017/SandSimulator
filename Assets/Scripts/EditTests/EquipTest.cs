using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class EquipTest
//{
//    [Test]
//    public void Test()
//    {
//        IModifiers modifiers = new MockPlayerModeifier();
//        IEquipRepository equip = new MockDataBabse();
//        IPlayerEquipRepository playerEquip = new MockPlayerEquipDataBase();
//        var equipservice = new EquipService(modifiers,playerEquip);

//        equipservice.Equip("fa");
//        equipservice.Equip("fa");

//    }
//}

//public class MockPlayerEquipDataBase : IPlayerEquipRepository
//{
//    public void AddEquip(string equipId)
//    {
//        Debug.Log("‘•”õ");
//    }

//    public EquipData FindData(string equipId)
//    {
//        return null;
//    }

//    public EquipId FindEquipId(string equipId)
//    {
//        return new EquipId("Dada");
//    }

//    public void RemoveEquip(string equipId)
//    {
//        Debug.Log("‘•”õŠO‚·");
//    }
//}

//public class MockDataBabse : IEquipRepository
//{
//    public void AddEquip(string equipId)
//    {
//        Debug.Log("‘•”õ");
//    }

//    public EquipData FindData(string equipId)
//    {
//        return null;
//    }

//    public EquipId FindId(string equipId)
//    {
//        return new EquipId("equipId");
//    }

//    public void RemoveEquip(string equipId)
//    {
//        Debug.Log("‘•”õ‚ğŠO‚·");
//    }
//}

//public class MockPlayerModeifier : IModifiers
//{
//    public void AddModifiers(string id)
//    {
//        Debug.Log("UŒ‚—ÍUP");
//    }

//    public void RemoveModifiers(string id)
//    {
//        Debug.Log("UŒ‚—Ídown");
//    }
//}
