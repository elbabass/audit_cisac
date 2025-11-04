from enum import Enum

class HighWatermarkType(Enum):
    AUDITHIGHWATERMARK = 'AuditHighWaterMark'
    CHANGETRACKERWATERMARK = 'ChangeTrackerWatermark'