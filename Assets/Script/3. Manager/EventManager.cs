using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager: IManager
{
    // 이벤트 핸들러 델리게이트 정의
    public delegate void EventHandler<T>(T eventData);
    public delegate void EventHandler();
 
    // 매개변수가 있는 이벤트를 위한 딕셔너리
    private Dictionary<string, object> eventDictionary = new Dictionary<string, object>();
    
    // 매개변수가 없는 이벤트를 위한 딕셔너리
    private Dictionary<string, List<Action>> simpleEventDictionary = new Dictionary<string, List<Action>>();



    // 이벤트 우선순위 상수
    public enum Priority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        VeryHigh = 3
    }

    
    public void Init()
    {
        
    }

    public void Clear()
    {
        
    }


    // 이벤트 리스너 클래스 (우선순위 정보 포함)
    private class EventListener<T>
    {
        public EventHandler<T> Handler;
        public Priority Priority;

        public EventListener(EventHandler<T> handler, Priority priority)
        {
            Handler = handler;
            Priority = priority;
        }
    }



    #region 이벤트 등록/해제 - 매개변수 있음

    // 이벤트 리스너 등록 (매개변수 있음)
    public void AddListener<T>(string eventName, EventHandler<T> listener, Priority priority = Priority.Normal)
    {
        List<EventListener<T>> eventListeners;

        // 이벤트 존재 여부 확인
        if (eventDictionary.TryGetValue(eventName, out object value))
        {
            eventListeners = value as List<EventListener<T>>;
            
            // 타입 불일치 확인
            if (eventListeners == null)
            {
                Debug.LogError($"이벤트 '{eventName}'에 다른 타입의 리스너가 이미 등록되어 있습니다.");
                return;
            }
        }
        else
        {
            // 새 이벤트 리스트 생성
            eventListeners = new List<EventListener<T>>();
            eventDictionary[eventName] = eventListeners;
        }

        // 중복 등록 방지
        foreach (var existingListener in eventListeners)
        {
            if (existingListener.Handler.Equals(listener))
            {
                Debug.LogWarning($"이벤트 '{eventName}'에 이미 동일한 리스너가 등록되어 있습니다.");
                return;
            }
        }

        // 리스너 추가
        eventListeners.Add(new EventListener<T>(listener, priority));
        
        // 우선순위에 따라 정렬
        eventListeners.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    // 이벤트 리스너 제거 (매개변수 있음)
    public void RemoveListener<T>(string eventName, EventHandler<T> listener)
    {
        if (eventDictionary.TryGetValue(eventName, out object value))
        {
            List<EventListener<T>> eventListeners = value as List<EventListener<T>>;
            
            if (eventListeners != null)
            {
                for (int i = 0; i < eventListeners.Count; i++)
                {
                    if (eventListeners[i].Handler.Equals(listener))
                    {
                        eventListeners.RemoveAt(i);
                        break;
                    }
                }
                
                // 리스너가 없으면 이벤트 제거
                if (eventListeners.Count == 0)
                {
                    eventDictionary.Remove(eventName);
                }
            }
        }
    }

    // 이벤트 발행 (매개변수 있음)
    public void TriggerEvent<T>(string eventName, T eventData)
    {
        if (eventDictionary.TryGetValue(eventName, out object value))
        {
            List<EventListener<T>> eventListeners = value as List<EventListener<T>>;
            
            if (eventListeners != null)
            {
                // 복사본을 만들어서 이벤트 실행 중 리스너가 추가/제거되는 경우 대비
                List<EventListener<T>> listenersCopy = new List<EventListener<T>>(eventListeners);
                
                foreach (var listener in listenersCopy)
                {
                    try
                    {
                        listener.Handler(eventData);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"이벤트 '{eventName}' 처리 중 오류 발생: {e.Message}\n{e.StackTrace}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"이벤트 '{eventName}'에 등록된 리스너의 타입이 호환되지 않습니다.");
            }
        }
    }

    #endregion

    #region 이벤트 등록/해제 - 매개변수 없음

    // 이벤트 리스너 등록 (매개변수 없음)
    public void AddListener(string eventName, Action listener)
    {
        if (!simpleEventDictionary.TryGetValue(eventName, out List<Action> listeners))
        {
            listeners = new List<Action>();
            simpleEventDictionary[eventName] = listeners;
        }

        // 중복 등록 방지
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
        else
        {
            Debug.LogWarning($"이벤트 '{eventName}'에 이미 동일한 리스너가 등록되어 있습니다.");
        }
    }

    // 이벤트 리스너 제거 (매개변수 없음)
    public void RemoveListener(string eventName, Action listener)
    {
        if (simpleEventDictionary.TryGetValue(eventName, out List<Action> listeners))
        {
            listeners.Remove(listener);
            
            // 리스너가 없으면 이벤트 제거
            if (listeners.Count == 0)
            {
                simpleEventDictionary.Remove(eventName);
            }
        }
    }

    // 이벤트 발행 (매개변수 없음)
    public void TriggerEvent(string eventName)
    {
        if (simpleEventDictionary.TryGetValue(eventName, out List<Action> listeners))
        {
            // 복사본을 만들어서 이벤트 실행 중 리스너가 추가/제거되는 경우 대비
            List<Action> listenersCopy = new List<Action>(listeners);
            
            foreach (var listener in listenersCopy)
            {
                try
                {
                    listener.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"이벤트 '{eventName}' 처리 중 오류 발생: {e.Message}\n{e.StackTrace}");
                }
            }
        }
    }

    #endregion

    #region 유틸리티 메서드

    // 특정 이벤트의 리스너 개수 확인
    public int GetListenerCount(string eventName)
    {
        int count = 0;
        
        if (eventDictionary.TryGetValue(eventName, out object value))
        {
            if (value is IList<object> list)
            {
                count += list.Count;
            }
        }
        
        if (simpleEventDictionary.TryGetValue(eventName, out List<Action> listeners))
        {
            count += listeners.Count;
        }
        
        return count;
    }

    // 모든 이벤트 리스너 제거
    public void ClearAllListeners()
    {
        eventDictionary.Clear();
        simpleEventDictionary.Clear();
        Debug.Log("모든 이벤트 리스너가 제거되었습니다.");
    }

    // 특정 이벤트의 모든 리스너 제거
    public void ClearListeners(string eventName)
    {
        eventDictionary.Remove(eventName);
        simpleEventDictionary.Remove(eventName);
    }

    #endregion
}  