using System;
using System.Diagnostics;
using CaSessionUtilities;
using CaSessionUtilities.Wrapping;
using CaSessionUtilities.Wrapping.Implementation;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace CaSessionUtilitiesTest;

public class ResponseEncoderTests
{
    //Case 3
    //OID: 0.4.0.127.0.7.2.2.3.2.4
    //picPub: EC/308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //pcdPri: EC/308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B0201010428385F041F2D5F0E174792F5D7AEA8033155EBA069EEB8F8196FB00B58FA2A2EB8CE47D3591DC0F37CA082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A15403520004679F8EF23A726922FC74035F6AD17CCA37D1B24D214E250FBF5B89226780126BEF1A11F48C1671277C45ACA18B28256E689D7953A0AFF64D197A0B03063083A903B86A12F49380F1CE9C925473BD4FBF
    //File: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //Len: 64
    //Key alg: ECDH
    //SharedSecret: AFB822E8458293734E53752B41D4F596D1885A162B4E0CE9E693F71E6687CB5F6EF79839D34B5D2D
    //cipherAlg: AES 256
    //ksEnc: AES RAW 5E47B10A9FC6805C1E5B600E70B0160F71B3082066206D912806637933303545
    //ksMac: AES RAW 603B13DDA5B63D60BE2474D5F1D9D1C6C4FB7F0D2EC1E9D416A410F8B0DA035E
    //IV: FB4D9A013CE175B98ACEFA0426A612B3
    //Response       : 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606
    //Padded Response: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E3017060680000000000000000000000000000000
    //Ciphertext response:8A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE
    //Ciphertext response Size Block:5101
    //Result Do87: 8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE
    //Result Do99: 8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE99029000
    //MAC this:    8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE99029000
    //Result MAC : 8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE990290008E08D113F81A4F2EE74A
    //Result     : 8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE990290008E08D113F81A4F2EE74A9000
    //result:      8751018A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE990290008E08D113F81A4F2EE74A9000


    //Case 4
    //OID: 0.4.0.127.0.7.2.2.3.2.4
    //picPub: EC/308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //pcdPri: EC/308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B0201010428CD6608388084FC169C4EDA09C0EFF91E9C1C9EDE1EC44676E79E9AD99A224D72C10761121F39227AA082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A15403520004D2980C3660B2A98A67BE335378436A45DADEDD58C47A8A022C4B2E11ACC6F7B8467031EEBB351501063693D03793F8DBFB862D90B4332AC70363658CFF640D8BA5D2444CB3D40A00556A6E4123EC11FC
    //File: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //Len: 1
    //Key alg: ECDH
    //SharedSecret: 3EEFE437BAE7FF5AC162EF53FEB4E20E477E3902067FCD6F46AD54F06B94B223411F6F92FBFD4BD2
    //cipherAlg: AES 256
    //ksEnc: AES RAW 0B89852F4866E35BED4BB7B3C271742A7E73F179CEFFEDD1F18A69A59A04251C
    //ksMac: AES RAW D8F7140D5AA85134507F6D298B760A7CFC8F1C92C600BEC55E963BC5C4D4FC0D
    //IV: 89D03B1EAC8F6A9CACF59B52C4FDB38E
    //Response       : 6E
    //Padded Response: 6E800000000000000000000000000000
    //Ciphertext response:0C1A91577955228ADB5AA0F4FAADF15D
    //Ciphertext response Size Block:1101
    //Result Do87: 8711010C1A91577955228ADB5AA0F4FAADF15D
    //Result Do99: 8711010C1A91577955228ADB5AA0F4FAADF15D99029000
    //MAC this:    8711010C1A91577955228ADB5AA0F4FAADF15D99029000
    //Result MAC : 8711010C1A91577955228ADB5AA0F4FAADF15D990290008E08F702D6A477B6165A
    //Result     : 8711010C1A91577955228ADB5AA0F4FAADF15D990290008E08F702D6A477B6165A9000
    //result:      8711010C1A91577955228ADB5AA0F4FAADF15D990290008E08F702D6A477B6165A9000



    //picPub: EC/308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //pcdPri: EC/308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B0201010428202475E77FBD8704C7D6BEFED311BC3F89B9FB507FEB89BC0EAAAA8E4C91A6F2FF2243ED07A3446DA082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A1540352000438C86B359DAE3341C26348EB325C984EDB8CEB9A543E2F328D79B8745CF9EB8109BA03463322B51D100B702947EF4A0D000BB1F8C8F296D0703B09E02EAD349C974667CFB6E14CFD188B56B8EB3FDC55
    //File: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
    //Len: 15
    //Key alg: ECDH
    //SharedSecret: CC07D70D57E7E15482BBE9D73A440BDE97BBDE1F14B7EC948ACA3F9F3AA8A8A49FE4237EFEACD50D
    //cipherAlg: AES 256
    //ksEnc: AES RAW 9DFA50578F1F91EF61B0EA9C02AD7297570DAB7B4EBCAF40FE5F004296B89A73
    //ksMac: AES RAW 4B84CA4A4492F6190B07ED0E4A00F0AE7046E165C25C60C51D7C7025A04E250B
    //IV: A07DFC26413F9C01A8B4F33BBF8B2AA0
    //Response       : 6E8201D9318201D5300D060804007F
    //Padded Response: 6E8201D9318201D5300D060804007F80
    //Ciphertext response:0D9B7C00D2767DE181AD9616A528A985
    //Ciphertext response Size Block:1101
    //Result Do87: 8711010D9B7C00D2767DE181AD9616A528A985
    //Result Do99: 8711010D9B7C00D2767DE181AD9616A528A98599029000
    //MAC this:    8711010D9B7C00D2767DE181AD9616A528A98599029000
    //Result MAC : 8711010D9B7C00D2767DE181AD9616A528A985990290008E0801BC2DCC3686E4E7
    //Result     : 8711010D9B7C00D2767DE181AD9616A528A985990290008E0801BC2DCC3686E4E79000
    //result:      8711010D9B7C00D2767DE181AD9616A528A985990290008E0801BC2DCC3686E4E79000


    //From SPECI2014 passport
    private const string HexEncodedDg14 = "6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396";

    [InlineData("CF1D8F4D1450D3ADE8A44E8F5737E9DDCBE5CD614CE0147185510FD8C35AD12F", "C9C8E169D1FDDBC1A186AB98552A5F5C146D84315F3C8AB8A9DDE6AA5B64105F", 64, "87510132355228C6B16E4A06B8934E4C235067A5D6D3855FBF8B912559E25F4C8F78693425CFFF0AB906C8F1F4DE082EBCD20DD07B13BDDD0B47BA89EDB6C0778F95560EFD66CEE04E35C831442EE9A90FAA42990290008E086618B0A723E0D1769000")]
    [InlineData("0B89852F4866E35BED4BB7B3C271742A7E73F179CEFFEDD1F18A69A59A04251C", "D8F7140D5AA85134507F6D298B760A7CFC8F1C92C600BEC55E963BC5C4D4FC0D", 1, "8711010C1A91577955228ADB5AA0F4FAADF15D990290008E08F702D6A477B6165A9000")]
    [InlineData("9DFA50578F1F91EF61B0EA9C02AD7297570DAB7B4EBCAF40FE5F004296B89A73", "4B84CA4A4492F6190B07ED0E4A00F0AE7046E165C25C60C51D7C7025A04E250B", 15, "8711010D9B7C00D2767DE181AD9616A528A985990290008E0801BC2DCC3686E4E79000")]
    [Theory]
    public void Write(string ksEncString, string ksMacString, int requestedLength, string expectedWrappedResponse)
    {
        var ksEnc = Hex.Decode(ksEncString);
        var ksMac = Hex.Decode(ksMacString);
        var encoder = new ResponseEncoder(new AesSecureMessagingWrapper(ksEnc, ksMac));
        var result = encoder.Write(Arrays.CopyOf(Hex.Decode(HexEncodedDg14), requestedLength));

        Trace.WriteLine("Actual  : " + Hex.ToHexString(result));
        Trace.WriteLine("Expected: " + expectedWrappedResponse.ToLower());

        var encodedActual = Hex.ToHexString(result);

        Assert.StartsWith("87", encodedActual); //Start tag
        var substring = encodedActual.Substring(encodedActual.Length - 32); //9902 + 9000 + 8e0b + 8 bytes (= 16 char) + 9000 again
        Debug.WriteLine($"Mac and block delimiter extract: {substring}");
        Assert.StartsWith("990290008e08", substring); //End of data block, start of MAC
        Assert.EndsWith("9000", substring); //Ends of block
        
        //TODO length block at start

        Assert.Equal(Hex.Decode(expectedWrappedResponse), result);
    }


    [InlineData("6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606", 16, "6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E3017060680000000000000000000000000000000")]
    [Theory]
    public void ResponsePadding(string response, int blockSize, string expected)
    {
        var result = PaddingIso9797.GetPaddedArrayMethod1(Hex.Decode(response), blockSize);
        var resultHex = Hex.ToHexString(result);
        
        Trace.WriteLine("Actual  : " + resultHex);
        Trace.WriteLine("Expected: " + expected);
        Assert.Equal(expected, resultHex, true);
    }

    [InlineData("CF1D8F4D1450D3ADE8A44E8F5737E9DDCBE5CD614CE0147185510FD8C35AD12F", 64, "32355228C6B16E4A06B8934E4C235067A5D6D3855FBF8B912559E25F4C8F78693425CFFF0AB906C8F1F4DE082EBCD20DD07B13BDDD0B47BA89EDB6C0778F95560EFD66CEE04E35C831442EE9A90FAA42")]
    [InlineData("5E47B10A9FC6805C1E5B600E70B0160F71B3082066206D912806637933303545", 64, "8A7DC86AFB30218A3E97164D8FEE9DB9804F067B7AA0D50790E3A13BB31B1DED90E6243958651637E830961AAD2857F8BCA82286BD8C1F7898DB6FC226EAEB4C682A59EE4776418D0C94EFEC8CC078EE")]
    [Theory]
    public void WrapperCipherText(string ksEncHex, int length, string expected)
    {
        var wrapper = new AesSecureMessagingWrapper(Hex.Decode(ksEncHex), new byte[0]);
        var result = wrapper.GetEncodedDataForResponse(Arrays.CopyOf(Hex.Decode(HexEncodedDg14), length).GetPaddedArrayMethod1(wrapper.BlockSize), 2);
        var resultHex = Hex.ToHexString(result);
        Trace.WriteLine("Actual  : " + resultHex);
        Trace.WriteLine("Expected: " + expected);
        Assert.Equal(expected, resultHex, true);
    }

    //Case2
    [InlineData("CF1D8F4D1450D3ADE8A44E8F5737E9DDCBE5CD614CE0147185510FD8C35AD12F", "D5C42BAAD967323C0DEE53FA9C7B7B5B")]
    [Theory]
    public void Iv(string ksEncString, string expectedIvHex)
    {
        var ksEnc = Hex.Decode(ksEncString);
        var actual = Hex.ToHexString(Crypto.GetAesEcbNoPaddingCipherText(ksEnc, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 }));

        Trace.WriteLine($"Expected: {expectedIvHex}");
        Trace.WriteLine($"Actual: {actual}");

        Assert.Equal(expectedIvHex, actual, true);
    }

    [InlineData(1, "0201")]
    [InlineData(64, "4101")]
    [InlineData(65, "4201")]
    [InlineData(254, "81ff01")]
    [InlineData(255, "82010001")]
    [InlineData(256, "82010101")]
    [InlineData(1024, "82040101")]
    [Theory]
    public void Do87LengthEncoding(int length, string expected)
    {
        Assert.Equal(expected, Hex.ToHexString(ResponseEncoder.GetEncodedDo87Size(length)));
    }

    [InlineData(1, "0201")]
    [InlineData(64, "4101")]
    [InlineData(65, "4201")]
    [InlineData(254, "81ff01")]
    [InlineData(255, "82010001")]
    [InlineData(256, "82010101")]
    [InlineData(1024, "82040101")]
    [Theory]
    public void IV(int length, string expected)
    {
        Assert.Equal(expected, Hex.ToHexString(ResponseEncoder.GetEncodedDo87Size(length)));
    }

    [Fact]
    public void Main()
    {
        var protocolOid = "0.4.0.127.0.7.2.2.3.2.4";
        var picPub //`: EC/308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
            = "308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396";
        var pcdPri // EC/308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B0201010428CB70B654031B1343CB0FED5A0C31DCED6DC2CB0D7F69719E94462C4070818926B297E25BFE02B8E5A082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A1540352000455AEF12A16BBC26338672A98D0BDE43974CEB0A7343B830DB90745BFDA1ECFE9482C050261C05DBD8A5E2C1FB2E2035BD4B6B7E261E4E1BF91CB46A3B78B93A9B86EE93372B3A7D5E48C12CE10BE283C
            = "308202C70201003082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C593110201010482019F3082019B0201010428CB70B654031B1343CB0FED5A0C31DCED6DC2CB0D7F69719E94462C4070818926B297E25BFE02B8E5A082011430820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C59311020101A1540352000455AEF12A16BBC26338672A98D0BDE43974CEB0A7343B830DB90745BFDA1ECFE9482C050261C05DBD8A5E2C1FB2E2035BD4B6B7E261E4E1BF91CB46A3B78B93A9B86EE93372B3A7D5E48C12CE10BE283C";
        //File: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606678108010105020101060A04007F0007010104010330820184060904007F000702020102308201753082011D06072A8648CE3D020130820110020101303406072A8648CE3D0101022900D35E472036BC4FB7E13C785ED201E065F98FCFA6F6F40DEF4F92B9EC7893EC28FCD412B1F1B32E27305404283EE30B568FBAB0F883CCEBD46D3F3BB8A2A73513F5EB79DA66190EB085FFA9F492F375A97D860EB40428520883949DFDBC42D3AD198640688A6FE13F41349554B49ACC31DCCD884539816F5EB4AC8FB1F1A604510443BD7E9AFB53D8B85289BCC48EE5BFE6F20137D10A087EB6E7871E2A10A599C710AF8D0D39E2061114FDD05545EC1CC8AB4093247F77275E0743FFED117182EAA9C77877AAAC6AC7D35245D1692E8EE1022900D35E472036BC4FB7E13C785ED201E065F98FCFA5B68F12A32D482EC7EE8658E98691555B44C5931102010103520004710DA6DAB5B770920D3D4D6807B02A13059BEFB4926E2D00CFDE4B4471571473A582934BBE92059800663578C83419E3563FE3E8AF3AE58B521D3741693C9CE19B312392CB00F59AF086863186706396
        var requestedLength = 64;
        //Key alg: ECDH
        var SharedSecretHex = "7594D86478914FC615837C7662F5223756AFC802761C229D38B0EC9333E08DAE5B518FE958BB7ADC";
        //cipherAlg: AES 256
        var ksEnc = "82B08003E2903814FFE1E3F1BC1192021B3F2850F73B5A140E4DE9EE1653B084";
        var ksMac = "E27B7049E66DA750CB31FA147919DD3AF4069AF13B1F0117EDC57D34EF104B7E";
        //Response: 6E8201D9318201D5300D060804007F0007020202020101300F060A04007F000702020302040201013012060A04007F0007020204020402010202010E30170606
        //MAC this: 875101852279ED751DD25FEE4A566C781B1F80BCE57C3A276847A63D015E00E1865B3E9331A1754550F8A78E15F1575CD6D7B7B2EB442D13368C727512C3882DD5872C27669CE5C0145AED39E95B954122513299029000
        var result = "875101852279ED751DD25FEE4A566C781B1F80BCE57C3A276847A63D015E00E1865B3E9331A1754550F8A78E15F1575CD6D7B7B2EB442D13368C727512C3882DD5872C27669CE5C0145AED39E95B9541225132990290008E089120F0AB68E7F6FE9000";

        var alg = ChipAuthenticationInfo.GetKeyAgreementAlgorithm(protocolOid);
        Assert.Equal(KeyAgreementAlgorithm.ECDH, alg);

        var ss = EACCAProtocol.ComputeSharedSecret(alg, PublicKeyFactory.CreateKey(Hex.Decode(picPub)), PrivateKeyFactory.CreateKey(Hex.Decode(pcdPri)));
        Assert.Equal(Hex.Decode(SharedSecretHex), ss);

        var wrapper = EACCAProtocol.RestartSecureMessaging(protocolOid, ss);

        Assert.IsType<AesSecureMessagingWrapper>(wrapper);
        Assert.Equal(16, wrapper.BlockSize);
        Assert.Equal(Hex.Decode(ksEnc), wrapper.KsEnc);
        Assert.Equal(Hex.Decode(ksMac), wrapper.KsMac);
        Assert.Equal(256, wrapper.MaxTranceiveLength); //TODO could this be affecting MRTD decrypt result? YES! Because is is a parameter for the CommandApdu

        var encoder = new ResponseEncoder(wrapper);
        var encodedResponse = encoder.Write(Arrays.CopyOf(Hex.Decode(HexEncodedDg14), requestedLength));

        Trace.WriteLine($"Expected: {result}");
        Trace.WriteLine($"Actual  : {Hex.ToHexString(encodedResponse)}");

        Assert.Equal(Hex.Decode(result), encodedResponse);

    }

}