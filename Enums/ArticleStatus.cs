namespace NewsPage.Enums
{
    public enum ArticleStatus
    {
        DRAFT,      // Bản nháp
        PENDING,    // Đang chờ Admin duyệt
        PUBLISHED,  // Đã xuất bản
        REJECTED,   // Bị từ chối bởi Admin
        HIDDEN      // Đã PUBLISHED và bị ẩn
    }
}
