﻿using System;
using System.Diagnostics;
using System.Linq;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class Step1_SharedSecretGeneration
{
    [InlineData(Case1_PcdPrivateKey, SharedSecretCase1)]
    [InlineData(Case2_PcdPrivateKey, SharedSecretCase2)]
    [InlineData(Case3_PcdPrivateKey, SharedSecretCase3)]
    [InlineData(Case5_PcdPrivateKey, Case5_SharedSecret)]
    [Theory]
    public void GenerateSharedSecretTests(string pcdPrivateKey, string expectedSharedSecret)
    {
        var agreementAlg = ChipAuthenticationInfo.GetKeyAgreementAlgorithm(Spec2014Content.DG14_CA_ProtocolOID);
        Assert.Equal(KeyAgreementAlgorithm.ECDH, agreementAlg);

        var sharedSecret = EACCAProtocol.ComputeSharedSecret(agreementAlg, PublicKeyFactory.CreateKey(Hex.Decode(Spec2014Content.DG14_PubKeyHex)), PrivateKeyFactory.CreateKey(Hex.Decode(pcdPrivateKey)));

        Trace.WriteLine("Actual   : " + Hex.ToHexString(sharedSecret));
        Trace.WriteLine("Expected : " + expectedSharedSecret);

        Assert.Equal(Hex.Decode(expectedSharedSecret), sharedSecret);
    }

    //Case 1
    public const string Case1_PcdPrivateKey = "308202c70201003082011d06072a8648ce3d020130820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c593110201010482019f3082019b02010104284a8428731ae16673f4f864f7230e1de0426f713a305c3189c6a1007e4dbefdba23f8441945d5d4aaa082011430820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c59311020101a154035200046e8747e7d81e19c228dd2ab575457915ba379d7521069b9310196f4bbe17a209e38c4d83d26e9a530d37ed3340239c76c4c15b3a76df6391fabd04634d31c2f117cabb7361ec8af8ab91b04813f641ce";
    public const string SharedSecretCase1 = "b096e215d80bb7550180dcab03c106033c39dac10442d9307dec7bf5fffe81efbf6dc5243adc4ca0";

    //Case 2
    public const string Case2_PcdPrivateKey = "308202c70201003082011d06072a8648ce3d020130820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c593110201010482019f3082019b02010104289916a4a1d4442127746ac30f0c910d790d95a04d1db5998665a2211ba2415cb92836bce4e3ec41e5a082011430820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c59311020101a154035200049220c10660b521a948295cf341a8c2c4053cf82fc670801eca7fa0cbe3fe3b94e8aa7469fbbcd321637d239846bc3a205eca6da743fb92daf7b30933cf8cbdb2e32d7768fef49c042a913f3a6c1b3cc2";
    public const string SharedSecretCase2 = "95df400407cb4c6f66ae8d738f387bf95bfcfd299e65cdeb03af0b20a1b857301b29bd2dcea6e893";

    //Case 3
    public const string Case3_PcdPrivateKey = "308202c70201003082011d06072a8648ce3d020130820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c593110201010482019f3082019b0201010428365e1b8d63a0209b466ef6b5bc193cba3a1e7276bc30dd8c9ce0e9410cb32592da6127d1dedc1b5ea082011430820110020101303406072a8648ce3d0101022900d35e472036bc4fb7e13c785ed201e065f98fcfa6f6f40def4f92b9ec7893ec28fcd412b1f1b32e27305404283ee30b568fbab0f883ccebd46d3f3bb8a2a73513f5eb79da66190eb085ffa9f492f375a97d860eb40428520883949dfdbc42d3ad198640688a6fe13f41349554b49acc31dccd884539816f5eb4ac8fb1f1a604510443bd7e9afb53d8b85289bcc48ee5bfe6f20137d10a087eb6e7871e2a10a599c710af8d0d39e2061114fdd05545ec1cc8ab4093247f77275e0743ffed117182eaa9c77877aaac6ac7d35245d1692e8ee1022900d35e472036bc4fb7e13c785ed201e065f98fcfa5b68f12a32d482ec7ee8658e98691555b44c59311020101a15403520004cc5b4e8687659c97e92be269b4e1091a88f5f369bf0bd317e29cdb5122c9765b7c5e594ce836526fcf287cb87036244dd738763f2bd0bb91507f54b029ac8c6d694dbda1a998d1af0fbe1c4f65a356e2";
    public const string SharedSecretCase3 = "7394cf3eafff620eace9bc16461f9719e49e13fa6bf1221c0de724127a9fc471c57b37342e49afbe";

    public const string Case5_PcdPublicKey = "308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004B2695BFC6B9671B871D4658005496EDA45595C3062E91191D3E82923DBBE4DAFE27AB3D422A67E8605646F1BA5A78ED607F8616CEDD684212CC9A2AA5375340228CCEC6BA1AAFBA9B35F94EED224416E";
    public const string Case5_PcdPrivateKey = "308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B020101042837A867C1851C23261F0BA8017B1B430A9F91546B84F00FACBA5286B6C4A52D9AE61CBF31046B7F2CA082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A15403520004B2695BFC6B9671B871D4658005496EDA45595C3062E91191D3E82923DBBE4DAFE27AB3D422A67E8605646F1BA5A78ED607F8616CEDD684212CC9A2AA5375340228CCEC6BA1AAFBA9B35F94EED224416E";
    public const string Case5_SharedSecret = "ccec921c2f4332aebf55c232a925bf675c8a1efff9bde9e965aa12c8421fe1beedc7646ce2b76128";
}