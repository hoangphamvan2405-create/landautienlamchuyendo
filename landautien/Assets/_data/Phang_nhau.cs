using System;
using UnityEngine;

public class Phang_nhau : MonoBehaviour
{
	private NhanVat nguoiChoi;
	private NhanVat quaiVat;

	private void Awake()
	{
		nguoiChoi = new NhanVat("Người chơi", 100);
		quaiVat = new NhanVat("Quái vật", 100);

		Debug.Log($"Đã tạo {nguoiChoi.Ten} ({nguoiChoi.Mau} máu) và {quaiVat.Ten} ({quaiVat.Mau} máu).");
	}
}

public sealed class NhanVat
{
	public string Ten { get; }
	public int Mau { get; private set; }

	public NhanVat(string ten, int mauBanDau)
	{
		if (string.IsNullOrWhiteSpace(ten))
		{
			throw new ArgumentException("Tên nhân vật không được để trống.", nameof(ten));
		}

		if (mauBanDau <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(mauBanDau), "Máu ban đầu phải lớn hơn 0.");
		}

		Ten = ten;
		Mau = mauBanDau;
	}
}
