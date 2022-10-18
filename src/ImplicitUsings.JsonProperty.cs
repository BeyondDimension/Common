// C# 10 定义全局 using

#if __HAVE_N_JSON__
global using N_JsonIgnore = Newtonsoft.Json.JsonIgnoreAttribute;
global using N_JsonProperty = Newtonsoft.Json.JsonPropertyAttribute;
#endif
#if !__NOT_HAVE_S_JSON__
global using S_JsonIgnore = System.Text.Json.Serialization.JsonIgnoreAttribute;
global using S_JsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#endif