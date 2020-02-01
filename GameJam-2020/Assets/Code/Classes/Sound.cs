using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[Serializable]
public class Sound
{
    public string name;
    [EventRef, SerializeField]
    private string fmodFile;
    private EventInstance instance;
    [SerializeField]
    private List<ParamRef> parameters;

    public void Init(){
        if (fmodFile != null && fmodFile != "" && !instance.isValid())
        {
            instance = RuntimeManager.CreateInstance(fmodFile);
        }
    }

    #region Play/Stop
    //Funciones encargadas de reproducir el sonido y si no existe la instancia crearla
    public void Play(Transform transform, Rigidbody rigidbody)
    {
        if (instance.isValid())
        {
            instance.start();
            RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidbody);
        }
    }

    public void Play(Transform transform)
    {
        if (instance.isValid())
        {
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
            instance.start();
        }
    }

    //Funcion encargada de detener un sonido
    public void Stop(FMOD.Studio.STOP_MODE stopType)
    {
        if (instance.isValid() && isPlaying())
            instance.stop(stopType);
    }
    #endregion

    //Funciones encargadas de recibir los parametros y asignarlos al evento
    #region Parameters
    public void SetParameter(string name, float value)
    {
        if (instance.isValid())
        {
            ParamRef parameter = parameters.FirstOrDefault(p => p.Name == name);
            if (parameter != null)
            {
                parameter.Value = value;
                instance.setParameterValue(parameter.Name, parameter.Value);
            }
        }
    }

    public void SetParameters(Dictionary<string, float> _parameter)
    {
        if (instance.isValid())
            foreach (string parameterName in _parameter.Keys)
            {
                ParamRef parameter = parameters.FirstOrDefault(p => p.Name == name);
                if (parameter != null)
                {
                    parameter.Value = _parameter[parameter.Name];
                    instance.setParameterValue(parameter.Name, parameter.Value);
                }
            }
    }

    public void SetParameters(string[] _parameters, float[] values)
    {
        if (instance.isValid() && _parameters.Length == values.Length)
            for (int i = 0; i < _parameters.Length; i++)
            {
                ParamRef parameter = parameters.FirstOrDefault(p => p.Name == _parameters[i]);
                if (parameter != null)
                {
                    parameter.Value = values[i];
                    instance.setParameterValue(parameter.Name, parameter.Value);
                }
            }
    }

    public void SetParameters(List<string> _parameters, List<float> values)
    {
        if (instance.isValid() && _parameters.Count == values.Count)
            for (int i = 0; i < _parameters.Count; i++)
            {
                ParamRef parameter = parameters.FirstOrDefault(p => p.Name == _parameters[i]);
                if (parameter != null)
                {
                    parameter.Value = values[i];
                    instance.setParameterValue(parameter.Name, parameter.Value);
                }
            }
    }

    public ParamRef[] GetParameters()
    {
        return parameters.ToArray();
    }

    public ParamRef GetParameter(string name)
    {
        return parameters.FirstOrDefault(p => p.Name == name);
    }

    public string[] GetParameterNames()
    {
        return parameters.Select(p => p.Name).ToArray();
    }
    #endregion

    #region Methods
    public bool isPlaying()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);

        return (state == FMOD.Studio.PLAYBACK_STATE.PLAYING) || (state == FMOD.Studio.PLAYBACK_STATE.STARTING);
    }
    #endregion
}