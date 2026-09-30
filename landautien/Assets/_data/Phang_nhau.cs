using System;
using System.Collections;
using UnityEngine;

public class Phang_nhau : MonoBehaviour
{
	[SerializeField] private int mauNguoiChoi = 100;
	[SerializeField] private int satThuongNguoiChoi = 20;
	[SerializeField] private int mauQuaiVat = 20;
	[SerializeField] private int satThuongQuaiVat = 5;
	[SerializeField] private int luongHoiMau = 20;

	private NhanVat nguoiChoi;
	private NhanVat quaiVat;
	private bool dangChienDau;
	private bool dangChonHanhDong;
	private bool daChonHanhDong;
	private HanhDong hanhDongNguoiChoi;

	private void Awake()
	{
		try
		{
			nguoiChoi = new NhanVat("Hoangkfczzzz", mauNguoiChoi, satThuongNguoiChoi, luongHoiMau);
			quaiVat = new NhanVat("Slime", mauQuaiVat, satThuongQuaiVat, luongHoiMau);
			Debug.Log($"Đã tạo {nguoiChoi.Ten} ({nguoiChoi.Mau} máu) và {quaiVat.Ten} ({quaiVat.Mau} máu).");
		}
		catch (ArgumentException exception)
		{
			Debug.LogError($"Không thể tạo nhân vật: {exception.Message}");
		}
	}

	private void BatDauTranDau()
	{
		if (dangChienDau || nguoiChoi == null || quaiVat == null)
		{
			return;
		}

		nguoiChoi.DatLaiMau();
		quaiVat.DatLaiMau();
		StartCoroutine(VongLapTranDau());
	}

	private IEnumerator VongLapTranDau()
	{
		dangChienDau = true;
		int soLuot = 1;
		Debug.Log("Trận đấu bắt đầu. Người chơi được đánh trước.");

		while (nguoiChoi.ConSong && quaiVat.ConSong)
		{
			Debug.Log($"--- Lượt {soLuot}: {nguoiChoi.Ten} ---");
			dangChonHanhDong = true;
			daChonHanhDong = false;
			while (!daChonHanhDong)
			{
				yield return null;
			}
			dangChonHanhDong = false;

			ThucHienHanhDong(nguoiChoi, quaiVat, hanhDongNguoiChoi);
			InMauHienTai();
			if (!quaiVat.ConSong)
			{
				break;
			}

			Debug.Log($"--- Lượt {soLuot}: {quaiVat.Ten} ---");
			HanhDong hanhDongQuaiVat = (HanhDong)UnityEngine.Random.Range(0, 3);
			ThucHienHanhDong(quaiVat, nguoiChoi, hanhDongQuaiVat);
			InMauHienTai();
			soLuot++;
			yield return new WaitForSeconds(0.5f);
		}

		dangChonHanhDong = false;
		dangChienDau = false;
		Debug.Log(nguoiChoi.ConSong ? "Người chơi chiến thắng!" : "Quái vật chiến thắng!");
	}

	private void ThucHienHanhDong(NhanVat nguoiHanhDong, NhanVat mucTieu, HanhDong hanhDong)
	{
		try
		{
			switch (hanhDong)
			{
				case HanhDong.TanCong:
					nguoiHanhDong.TanCong(mucTieu);
					break;
				case HanhDong.PhongThu:
					nguoiHanhDong.PhongThu();
					break;
				case HanhDong.HoiMau:
					nguoiHanhDong.HoiMau();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(hanhDong), "Hành động không hợp lệ.");
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	private void InMauHienTai()
	{
		Debug.Log($"Máu hiện tại | {nguoiChoi.Ten}: {nguoiChoi.Mau}/{nguoiChoi.MauToiDa} | " +
		          $"{quaiVat.Ten}: {quaiVat.Mau}/{quaiVat.MauToiDa}");
	}

	private void ChonHanhDong(HanhDong hanhDong)
	{
		if (!dangChonHanhDong || daChonHanhDong)
		{
			return;
		}

		hanhDongNguoiChoi = hanhDong;
		daChonHanhDong = true;
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(20, 20, 280, 300), GUI.skin.box);
		GUILayout.Label("Trận đấu");

		if (nguoiChoi == null || quaiVat == null)
		{
			GUILayout.Label("Không thể tạo nhân vật. Kiểm tra chỉ số trong Inspector.");
			GUILayout.EndArea();
			return;
		}

		GUILayout.Label($"{nguoiChoi.Ten}: {nguoiChoi.Mau}/{nguoiChoi.MauToiDa} HP");
		GUILayout.Label($"{quaiVat.Ten}: {quaiVat.Mau}/{quaiVat.MauToiDa} HP");

		if (!dangChienDau && nguoiChoi.ConSong && quaiVat.ConSong)
		{
			if (GUILayout.Button("Bắt đầu trận đấu"))
			{
				BatDauTranDau();
			}
		}
		else if (dangChonHanhDong)
		{
			GUILayout.Label("Chọn hành động:");
			if (GUILayout.Button("1. Tấn công")) ChonHanhDong(HanhDong.TanCong);
			if (GUILayout.Button("2. Phòng thủ")) ChonHanhDong(HanhDong.PhongThu);
			if (GUILayout.Button("3. Hồi máu")) ChonHanhDong(HanhDong.HoiMau);
		}
		else if (!dangChienDau)
		{
			if (GUILayout.Button("Chơi lại"))
			{
				BatDauTranDau();
			}
		}

		GUILayout.EndArea();
	}
}

public enum HanhDong
{
	TanCong,
	PhongThu,
	HoiMau
}

public sealed class NhanVat
{
	private readonly int satThuongGoc;
	private readonly int luongHoiMau;
	private bool dangPhongThu;

	public string Ten { get; }
	public int Mau { get; private set; }
	public int MauToiDa { get; }
	public bool ConSong => Mau > 0;

	public NhanVat(string ten, int mauBanDau, int satThuongGoc, int luongHoiMau)
	{
		if (string.IsNullOrWhiteSpace(ten))
		{
			throw new ArgumentException("Tên nhân vật không được để trống.", nameof(ten));
		}

		if (mauBanDau <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(mauBanDau), "Máu ban đầu phải lớn hơn 0.");
		}
		if (satThuongGoc <= 0) throw new ArgumentOutOfRangeException(nameof(satThuongGoc), "Sát thương phải lớn hơn 0.");
		if (luongHoiMau <= 0) throw new ArgumentOutOfRangeException(nameof(luongHoiMau), "Lượng hồi máu phải lớn hơn 0.");

		Ten = ten;
		Mau = mauBanDau;
		MauToiDa = mauBanDau;
		this.satThuongGoc = satThuongGoc;
		this.luongHoiMau = luongHoiMau;
	}

	public void TanCong(NhanVat mucTieu)
	{
		KiemTraConSong();
		if (mucTieu == null) throw new ArgumentNullException(nameof(mucTieu));
		if (!mucTieu.ConSong) throw new InvalidOperationException($"{mucTieu.Ten} đã hết máu.");

		int satThuong = Mathf.RoundToInt(satThuongGoc * UnityEngine.Random.Range(0.8f, 1.2f));
		int satThuongThucTe = mucTieu.NhanSatThuong(satThuong);
		Debug.Log($"{Ten} tấn công {mucTieu.Ten}, gây {satThuongThucTe} sát thương.");
	}

	public void PhongThu()
	{
		KiemTraConSong();
		dangPhongThu = true;
		Debug.Log($"{Ten} phòng thủ, giảm một nửa sát thương từ đòn đánh tiếp theo.");
	}

	public void HoiMau()
	{
		KiemTraConSong();
		int mauTruocKhiHoi = Mau;
		Mau = Mathf.Min(MauToiDa, Mau + luongHoiMau);
		Debug.Log($"{Ten} hồi {Mau - mauTruocKhiHoi} máu.");
	}

	public void DatLaiMau()
	{
		Mau = MauToiDa;
		dangPhongThu = false;
	}

	private int NhanSatThuong(int satThuong)
	{
		int satThuongThucTe = dangPhongThu ? Mathf.CeilToInt(satThuong / 2f) : satThuong;
		dangPhongThu = false;
		Mau = Mathf.Max(0, Mau - satThuongThucTe);
		return satThuongThucTe;
	}

	private void KiemTraConSong()
	{
		if (!ConSong)
		{
			throw new InvalidOperationException($"{Ten} không thể hành động khi đã hết máu.");
		}
	}
}
