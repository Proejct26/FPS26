using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class JobQueue : Singleton<JobQueue>
{
    private readonly ConcurrentQueue<Action> _jobs = new ConcurrentQueue<Action>();
    private readonly object _lock = new object();
    private volatile bool _isProcessing = false;
    private const int MAX_JOBS_PER_FRAME = 100; // 프레임당 최대 처리할 작업 수
    private const float MAX_PROCESSING_TIME = 0.016f; // 16ms (약 60fps)

    public static void Push(Action job)
    {
        if (job == null) return;
        
        Instance._jobs.Enqueue(job);
    }
  
    private void Update()
    {
        if (_isProcessing) return;
         
        ProcessJobs(); 
    }

    private void ProcessJobs()
    {
        _isProcessing = true;
        try
        {
            float startTime = Time.realtimeSinceStartup;
            int processedCount = 0;


            while (_jobs.TryDequeue(out Action job) && 
                   processedCount < MAX_JOBS_PER_FRAME && 
                   Time.realtimeSinceStartup - startTime < MAX_PROCESSING_TIME)
            {
                try
                {
                    job?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Job execution error: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    processedCount++;
                }
            }   

            // 남은 작업이 있다면 다음 프레임에서 처리
            if (!_jobs.IsEmpty)
            {
                Push(() => { }); // 빈 작업을 추가하여 다음 프레임에서 계속 처리
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }

    // 현재 대기 중인 작업 수를 반환
    public static int GetPendingJobCount()
    {
        return Instance._jobs.Count;
    }

    // 모든 작업을 즉시 처리
    public static void ProcessAllJobs()
    {
        Instance.ProcessAllJobsInternal();
    }

    private void ProcessAllJobsInternal()
    {
        _isProcessing = true;
        try
        {
            while (_jobs.TryDequeue(out Action job))
            {
                try
                {
                    job?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Job execution error: {e.Message}\n{e.StackTrace}");
                }
            }
        }
        finally
        {
            _isProcessing = false;
        }
    }
}
