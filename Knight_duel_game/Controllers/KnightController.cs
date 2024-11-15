using Microsoft.AspNetCore.Mvc;

namespace Knight_duel_game.Controllers
{
    public class KnightController : Controller
    {
         {
        /*
         * Titanscontroller controls all functions for titans, including, missions.
         */

        private readonly Knight_duel_game _context;
        private readonly ITitansServices _titansServices;
        private readonly IFileServices _fileServices;

        public TitansController(GalacticTitansContext context, ITitansServices titansServices, IFileServices fileServices)
        {
            _context = context;
            _titansServices = titansServices;
            _fileServices = fileServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var resultingInventory = _context.Titans
                .OrderByDescending(y => y.TitanLevel)
                .Select(x => new TitanIndexViewModel
                {
                    ID = x.ID,
                    TitanName = x.TitanName,
                    TitanType = (Models.Titans.TitanType)(Core.Dto.TitanType)x.TitanType,
                    TitanLevel = x.TitanLevel,
                });
            return View(resultingInventory);
        }
        [HttpGet]
        public IActionResult Create()
        {
            TitanCreateViewModel vm = new();
            return View("Create", vm);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TitanCreateViewModel vm)
        {
            var dto = new TitanDto()
            {
                TitanName = vm.TitanName,
                TitanHealth = 100,
                TitanXP = 0,
                TitanXPNextLevel = 100,
                TitanLevel = 0,
                TitanType = (Core.Dto.TitanType)vm.TitanType,
                TitanStatus = (Core.Dto.TitanStatus)vm.TitanStatus,
                PrimaryAttackName = vm.PrimaryAttackName,
                PrimaryAttackPower = vm.PrimaryAttackPower,
                SecondaryAttackName = vm.SecondaryAttackName,
                SecondaryAttackPower = vm.SecondaryAttackPower,
                SpecialAttackName = vm.SpecialAttackName,
                SpecialAttackPower = vm.SpecialAttackPower,
                TitanWasBorn = vm.TitanWasBorn,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Files = vm.Files,
                Image = vm.Image
                .Select(x => new FileToDatabaseDto
                {
                    ID = x.ImageID,
                    ImageData = x.ImageData,
                    ImageTitle = x.ImageTitle,
                    TitanID = x.TitanID,
                }).ToArray()
            };
            var result = await _titansServices.Create(dto);

            if (result == null)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", vm);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id /*, Guid ref*/)
        {
            var titan = await _titansServices.DetailsAsync(id);

            if (titan == null)
            {
                return NotFound(); // <- TODO; custom partial view with message, titan is not located
            }

            var images = await _context.FilesToDatabase
                .Where(t => t.TitanID == id)
                .Select(y => new TitanImageViewModel
                {
                    TitanID = y.ID,
                    ImageID = y.ID,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();

            var vm = new TitanDetailsViewModel();
            vm.ID = titan.ID;
            vm.TitanName = titan.TitanName;
            vm.TitanHealth = titan.TitanHealth;
            vm.TitanXP = titan.TitanXP;
            vm.TitanLevel = titan.TitanLevel;
            vm.TitanType = (Models.Titans.TitanType)titan.TitanType;
            vm.TitanStatus = (Models.Titans.TitanStatus)titan.TitanStatus;
            vm.PrimaryAttackName = titan.PrimaryAttackName;
            vm.PrimaryAttackPower = titan.PrimaryAttackPower;
            vm.SecondaryAttackName = titan.SecondaryAttackName;
            vm.SecondaryAttackPower = titan.SecondaryAttackPower;
            vm.SpecialAttackName = titan.SpecialAttackName;
            vm.SpecialAttackPower = titan.SpecialAttackPower;
            vm.Image.AddRange(images);

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            if (id == null) { return NotFound(); }

            var titan = await _titansServices.DetailsAsync(id);

            if (titan == null) { return NotFound(); }

            var images = await _context.FilesToDatabase
                .Where(x => x.TitanID == id)
                .Select(y => new TitanImageViewModel
                {
                    TitanID = y.ID,
                    ImageID = y.ID,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();

            var vm = new TitanCreateViewModel();
            vm.ID = titan.ID;
            vm.TitanName = titan.TitanName;
            vm.TitanHealth = titan.TitanHealth;
            vm.TitanXP = titan.TitanXP;
            vm.TitanXPNextLevel = titan.TitanXPNextLevel;
            vm.TitanLevel = titan.TitanLevel;
            vm.TitanType = (Models.Titans.TitanType)titan.TitanType;
            vm.TitanStatus = (Models.Titans.TitanStatus)titan.TitanStatus;
            vm.PrimaryAttackName = titan.PrimaryAttackName;
            vm.PrimaryAttackPower = titan.PrimaryAttackPower;
            vm.SecondaryAttackName = titan.SecondaryAttackName;
            vm.SecondaryAttackPower = titan.SecondaryAttackPower;
            vm.SpecialAttackName = titan.SpecialAttackName;
            vm.SpecialAttackPower = titan.SpecialAttackPower;
            vm.TitanDied = titan.TitanDied;
            vm.TitanWasBorn = titan.TitanWasBorn;
            vm.CreatedAt = titan.CreatedAt;
            vm.UpdatedAt = DateTime.Now;
            vm.Image.AddRange(images);

            return View("Update", vm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(TitanCreateViewModel vm)
        {
            var dto = new TitanDto()
            {
                ID = (Guid)vm.ID,
                TitanName = vm.TitanName,
                TitanHealth = 100,
                TitanXP = 0,
                TitanXPNextLevel = 100,
                TitanLevel = 0,
                TitanType = (Core.Dto.TitanType)vm.TitanType,
                TitanStatus = (Core.Dto.TitanStatus)vm.TitanStatus,
                PrimaryAttackName = vm.PrimaryAttackName,
                PrimaryAttackPower = vm.PrimaryAttackPower,
                SecondaryAttackName = vm.SecondaryAttackName,
                SecondaryAttackPower = vm.SecondaryAttackPower,
                SpecialAttackName = vm.SpecialAttackName,
                SpecialAttackPower = vm.SpecialAttackPower,
                TitanWasBorn = vm.TitanWasBorn,
                CreatedAt = vm.CreatedAt,
                UpdatedAt = DateTime.Now,
                Files = vm.Files,
                Image = vm.Image
                .Select(x => new FileToDatabaseDto
                {
                    ID = x.ImageID,
                    ImageData = x.ImageData,
                    ImageTitle = x.ImageTitle,
                    TitanID = x.TitanID,
                }).ToArray()
            };
            var result = await _titansServices.Update(dto);

            if (result == null) { return RedirectToAction("Index"); }
            return RedirectToAction("Index", vm);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null) { return NotFound(); }

            var titan = await _titansServices.DetailsAsync(id);

            if (titan == null) { return NotFound(); };

            var images = await _context.FilesToDatabase
                .Where(x => x.TitanID == id)
                .Select(y => new TitanImageViewModel
                {
                    TitanID = y.ID,
                    ImageID = y.ID,
                    ImageData = y.ImageData,
                    ImageTitle = y.ImageTitle,
                    Image = string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(y.ImageData))
                }).ToArrayAsync();
            var vm = new TitanDeleteViewModel();

            vm.ID = titan.ID;
            vm.TitanName = titan.TitanName;
            vm.TitanHealth = titan.TitanHealth;
            vm.TitanXP = titan.TitanXP;
            vm.TitanXPNextLevel = titan.TitanXPNextLevel;
            vm.TitanLevel = titan.TitanLevel;
            vm.TitanType = (Models.Titans.TitanType)titan.TitanType;
            vm.TitanStatus = (Models.Titans.TitanStatus)titan.TitanStatus;
            vm.PrimaryAttackName = titan.PrimaryAttackName;
            vm.PrimaryAttackPower = titan.PrimaryAttackPower;
            vm.SecondaryAttackName = titan.SecondaryAttackName;
            vm.SecondaryAttackPower = titan.SecondaryAttackPower;
            vm.SpecialAttackName = titan.SpecialAttackName;
            vm.SpecialAttackPower = titan.SpecialAttackPower;
            vm.CreatedAt = titan.CreatedAt;
            vm.UpdatedAt = DateTime.Now;
            vm.Image.AddRange(images);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var titanToDelete = await _titansServices.Delete(id);

            if (titanToDelete == null) { return RedirectToAction("Index"); }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImage(TitanImageViewModel vm)
        {
            var dto = new FileToDatabaseDto()
            {
                ID = vm.ImageID
            };
            var image = await _fileServices.RemoveImageFromDatabase(dto);
            if (image == null) { return RedirectToAction("Index"); }
            return RedirectToAction("Index");
        }
    }
}